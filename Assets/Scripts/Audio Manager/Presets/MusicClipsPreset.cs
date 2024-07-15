using System.Collections.Generic;
using UnityEngine;

namespace MusicClips
{
    public enum LoopOption { Menu, Room, Store, CatRoom, PetTutorial, PaintTutorial }


    [CreateAssetMenu(fileName = "new MusicClipsPreset", menuName = "Custom/ClipsPreset/MusicClipsPreset", order = 1)]
    [System.Serializable]
    public class MusicClipsPreset : ScriptableObject
    {
        public AudioClip scoreSummaryMusicClip;

        [SerializeField] List<AudioClip> menuClips;
        [SerializeField] List<AudioClip> roomClips;
        [SerializeField] List<AudioClip> storeClips;
        [SerializeField] List<AudioClip> catRoomClips;

        [Header("Tutorial")]
        public AudioClip petTutorialClip;
        public AudioClip paintTutorialClip;

        public static LoopOption CurrentLoop { get; set; }

        // start in -1 so you can do clipIndex++ at first
        static int clipIndex = -1;

        public static bool RandomizeIndex { get; set; }

        public AudioClip Next()
        {
            // get clip list from current loop option
            List<AudioClip> loop = GetLoopList(CurrentLoop);

            // no mainloop sequence
            if (loop == null || loop.Count == 0)
                return null;

            // return a random clip
            if (RandomizeIndex)
                return loop[Random.Range(0, loop.Count)];

            clipIndex++;
            if (clipIndex > loop.Count - 1)
                clipIndex = 0;

            return loop[clipIndex];
        }

        public void RestartLoop()
        {
            clipIndex = -1;
        }

        public List<AudioClip> GetLoopList(LoopOption loopOption)
        {
            switch (loopOption)
            {
                case LoopOption.Menu:
                    return menuClips;
                case LoopOption.Room:
                    return roomClips;
                case LoopOption.Store:
                    return storeClips;
                case LoopOption.CatRoom:
                    return catRoomClips;

                case LoopOption.PetTutorial:
                    return new() { petTutorialClip };
                case LoopOption.PaintTutorial:
                    return new() { paintTutorialClip };

                default:
                    break;
            }

            return null;
        }
    }

}
