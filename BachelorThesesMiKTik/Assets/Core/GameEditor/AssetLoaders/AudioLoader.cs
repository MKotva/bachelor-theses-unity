using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Scripts.GameEditor.Audio;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Core.GameEditor.AssetLoaders
{
    public static class AudioLoader
    {
        /// <summary>
        /// Loads a audio clip via LoadAudioClip(). Audio clip and settings will be assigned to a given audio source.
        /// </summary>
        /// <param name="ob">Object for set.</param>
        /// <param name="url">Path to source.</param>
        /// <param name="xSize">X scale size.</param>
        /// <param name="ySize">Y scale size.</param>
        /// <returns></returns>
        public static async Task<bool> SetAudioClip(AudioSource source, AudioSourceDTO sourceDTO)
        {
            if(sourceDTO.URL == "")
                return false;

            var audioClip = await LoadAudioClip(sourceDTO.URL);
            if (audioClip == null)
            {
                return false;
            }
            source.clip = audioClip;
            source.priority = sourceDTO.Priority;
            source.panStereo = sourceDTO.StereoPan;
            source.pitch = sourceDTO.Pitch;
            source.volume = sourceDTO.Volume;
            source.loop = sourceDTO.ShouldLoop;
            return true;
        }

        /// <summary>
        /// Loads audio clip from a given url. If the url does not contains /, method will try obtain the file from local folder (Streaming assets).
        /// </summary>
        /// <param name="url">Path to source</param>
        /// <returns></returns>
        public static async Task<AudioClip> LoadAudioClip(string url)
        {
            if (!url.Contains('/'))
            {
                url = GetAssetPath(url);
            }

            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                request.certificateHandler = new Certificator();
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Delay(10);

                if (request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    OutputManager.Instance.ShowMessage(request.error);
                    return null;
                }

                var audio = DownloadHandlerAudioClip.GetContent(request);
                return audio;
            }
        }

        /// <summary>
        /// Creates path to streaming asset folder.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private static string GetAssetPath(string relativePath)
        {
            return "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
        }
    }
}
