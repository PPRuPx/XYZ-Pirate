using System;
using System.Collections.Generic;
using System.Linq;
using Model.Definitions.Player;
using UnityEngine;

namespace Model.Data
{
    [Serializable]
    public class LevelData
    {
        [SerializeField] private List<LevelProgress> _progress;

        public int GetLevel(StatId id)
        {
            var progress = FindLevelProgress(id);
            return progress?.Level ?? 0;
        }

        public void LevelUp(StatId id)
        {
            var progress = FindLevelProgress(id);
            if (progress == null)
                _progress.Add(new LevelProgress(id, 1));
            else
                progress.Level++;
        }
        
        private LevelProgress FindLevelProgress(StatId id)
        {
            LevelProgress progress = null;
            foreach (var x in _progress)
            {
                if (x.Id == id)
                {
                    progress = x;
                    break;
                }
            }

            return progress;
        }
    }

    [Serializable]
    public class LevelProgress
    {
        public StatId Id;
        public int Level;

        public LevelProgress(StatId id, int level)
        {
            Id = id;
            Level = level;
        }
    }
}