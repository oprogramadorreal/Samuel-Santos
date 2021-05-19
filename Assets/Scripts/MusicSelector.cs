using System.Collections.Generic;
using UnityEngine;

namespace ss
{
    public sealed class MusicSelector : MonoBehaviour
    {
        private static readonly IList<MusicEntry> musics = new List<MusicEntry>()
        {
            new MusicEntry { musicName = "MusicClassic", musicText = "Classic" },
            new MusicEntry { musicName = "MusicRock", musicText = "Rock" },
            new MusicEntry { musicName = "MusicElectronic", musicText = "Electronic" },
            new MusicEntry { musicName = "MusicClassic2", musicText = "Classic 2" },
        };

        private static int currentMusicIndex = 0;

        private const string MusicIndexPref = "CurrentMusicIndex";

        public static string SelectedMusicName
        {
            get
            {
                return musics[currentMusicIndex].musicName;
            }
        }

        public static string SelectedMusicText
        {
            get
            {
                return musics[currentMusicIndex].musicText;
            }
        }

        private void Awake()
        {
            SetCurrentMusic(PlayerPrefs.GetInt(MusicIndexPref, 0));
        }

        public static void SelectNextMusic()
        {
            SetCurrentMusic((currentMusicIndex + 1) % musics.Count);
        }

        private static void SetCurrentMusic(int musicIndex)
        {
            currentMusicIndex = musicIndex;
            PlayerPrefs.SetInt(MusicIndexPref, currentMusicIndex);
        }

        private struct MusicEntry
        {
            public string musicName;
            public string musicText;
        }
    }
}
