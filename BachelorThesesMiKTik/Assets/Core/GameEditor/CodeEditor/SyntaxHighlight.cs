using Assets.Core.GameEditor.Compile;
using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor
{
    public static class SyntaxHighlight
    {
        public static void ColorKeyWords(TMP_Text text)
        {
            var keyWords = HighlightKeyWords(text.text);
            foreach (var word in keyWords)
            {
                ColorChange(text, word);
            }

            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private static List<KeyWordDTO> HighlightKeyWords(string text)
        {
            var keyWord = new List<KeyWordDTO>();
            foreach (Match keyWordMatch in Patterns.KeyWordRegex.Matches(text))
            {
                keyWord.Add(new KeyWordDTO(keyWordMatch.Index, keyWordMatch.Length, new Color32(0, 0, 255, 255)));
            }

            foreach (Match valueTypeMatch in Patterns.TypeKeyWordRegex.Matches(text))
            {
                keyWord.Add(new KeyWordDTO(valueTypeMatch.Index, valueTypeMatch.Length, new Color32(207, 52, 118, 255)));
            }

            return keyWord;
        }

        private static void ColorChange(TMP_Text text, KeyWordDTO keyWord)
        {
            for (int i = keyWord.Index; i < keyWord.Index + keyWord.Lenght; ++i)
            {
                var meshIndex = text.textInfo.characterInfo[i].materialReferenceIndex;
                var vertexIndex = text.textInfo.characterInfo[i].vertexIndex;

                Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
                SetVertexColors(vertexColors, keyWord.Color, vertexIndex);
            }
        }

        private static void SetVertexColors(Color32[] colors, Color32 newColor, int vertexIndex)
        {
            for (int i = 0; i < 4; i++)
                colors[vertexIndex + i] = newColor;
        }
    }
}
