using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

// Troca marcas como {interact} pelo icone certo do botao, conforme o ultimo
// dispositivo usado (teclado e mouse, ou controle Xbox / PlayStation / Switch).
// Gera tags do TextMeshPro: <sprite name="kb_space_f">. Os nomes dos icones
// estao em Assets/Art/UI/InputPrompts/input_prompts_names.txt (sufixo _f =
// preenchido, _o = contornado).
//
// Para o TextMeshPro achar os icones: em Project Settings > TextMeshPro >
// Settings, arraste o asset "input_prompts" para o campo "Default Sprite Asset".
//
// Marcas disponiveis:
//   {interact}                 pegar / encaixar / avancar (Espaco, A, Cruz)
//   {rotate_left} {rotate_right} {rotate}   girar o bloco (Q E / LB RB / L1 R1)
//   {move}                     andar com o Al (WASD / analogico)
//   {finn_follow}              Finn seguir (clique esquerdo / analogico)
//   {finn_wheel}               roda de operadores (clique direito / botao de acao)
//   {swap}                     trocar de personagem no controle (Y / Triangulo / X do Switch).
//                              No teclado nao existe: a marca some (use o texto de controle da fala)
//   {mouse_left} {mouse_right} {pause}
public static class InputPromptResolver
{
    public enum Device
    {
        KeyboardMouse,
        Xbox,
        PlayStation,
        Switch
    }

    private static readonly Regex TokenPattern = new Regex(@"\{(\w+)\}", RegexOptions.Compiled);
    private static readonly HashSet<string> WarnedTokens = new HashSet<string>();

    private static Device _current = Device.KeyboardMouse;

    // Disparado quando o jogador passa de teclado para controle (ou entre tipos de controle).
    public static event Action DeviceChanged;

    public static Device Current => _current;
    public static bool IsGamepad => _current != Device.KeyboardMouse;

    public static void SetDevice(Device device)
    {
        if (device == _current)
        {
            return;
        }

        _current = device;
        DeviceChanged?.Invoke();
    }

    // iconSizePercent: 100 = tamanho natural do TextMeshPro; maior aumenta o icone.
    public static string Resolve(string text, bool filled = true, int iconSizePercent = 100)
    {
        return Resolve(text, _current, filled, iconSizePercent);
    }

    public static string Resolve(string text, Device device, bool filled, int iconSizePercent = 100)
    {
        if (string.IsNullOrEmpty(text) || text.IndexOf('{') < 0)
        {
            return text;
        }

        string style = filled ? "f" : "o";

        return TokenPattern.Replace(text, match =>
        {
            string token = match.Groups[1].Value;
            string result = ResolveToken(token, device, style, iconSizePercent);

            if (result == null)
            {
                if (WarnedTokens.Add(token))
                {
                    Debug.LogWarning("InputPromptResolver: marca desconhecida {" + token + "}");
                }

                return match.Value;
            }

            return result;
        });
    }

    private static string ResolveToken(string token, Device device, string style, int size)
    {
        bool pad = device != Device.KeyboardMouse;

        switch (token)
        {
            case "interact":
                return Icon(InteractName(device), style, size);

            case "rotate_left":
                return Icon(pad ? (device == Device.PlayStation ? "pad_l1" : "pad_l") : "kb_q", style, size);

            case "rotate_right":
                return Icon(pad ? (device == Device.PlayStation ? "pad_r1" : "pad_r") : "kb_e", style, size);

            case "rotate":
                return ResolveToken("rotate_left", device, style, size) + ResolveToken("rotate_right", device, style, size);

            case "move":
                return pad
                    ? Icon("pad_ls", style, size)
                    : Icon("kb_w", style, size) + Icon("kb_a", style, size) + Icon("kb_s", style, size) + Icon("kb_d", style, size);

            case "finn_follow":
                return pad ? Icon("pad_ls", style, size) : Icon("mouse_left", style, size);

            case "finn_wheel":
                return pad ? Icon(InteractName(device), style, size) : Icon("mouse_right", style, size);

            case "swap":
            case "swap_character":
                // Botao de cima do controle (o mesmo que troca Al <-> Finn no jogo).
                return pad ? Icon(SwapName(device), style, size) : string.Empty;

            case "mouse_left":
                return Icon("mouse_left", style, size);

            case "mouse_right":
                return Icon("mouse_right", style, size);

            case "pause":
                // O pacote de icones nao tem o botao Start: no controle vai em texto.
                return pad ? "[START]" : Icon("kb_esc", style, size);

            default:
                return null;
        }
    }

    private static string SwapName(Device device)
    {
        switch (device)
        {
            case Device.PlayStation: return "pad_triangle";
            case Device.Switch: return "pad_x"; // botao de cima do Switch
            default: return "pad_y";
        }
    }

    private static string InteractName(Device device)
    {
        switch (device)
        {
            case Device.Xbox: return "pad_a";
            case Device.PlayStation: return "pad_cross";
            case Device.Switch: return "pad_b"; // botao de baixo do Switch
            default: return "kb_space";
        }
    }

    private static string Icon(string baseName, string style, int size)
    {
        var sb = new StringBuilder();

        if (size != 100)
        {
            sb.Append("<size=").Append(size).Append("%>");
        }

        sb.Append("<sprite name=\"").Append(baseName).Append('_').Append(style).Append("\">");

        if (size != 100)
        {
            sb.Append("</size>");
        }

        return sb.ToString();
    }
}
