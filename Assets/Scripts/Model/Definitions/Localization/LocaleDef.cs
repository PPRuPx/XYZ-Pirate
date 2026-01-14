using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Model.Definitions.Localization
{
    // en - https://docs.google.com/spreadsheets/d/e/2PACX-1vQaEFU97vyhfSgaz-BhdN6sFIZvO-IR5dMbHOzF5lQt80oqCMKfLdZxQ0hcy7AdaMk0W7qJ_mm1-J7L/pub?gid=0&single=true&output=tsv
    // es - https://docs.google.com/spreadsheets/d/e/2PACX-1vQaEFU97vyhfSgaz-BhdN6sFIZvO-IR5dMbHOzF5lQt80oqCMKfLdZxQ0hcy7AdaMk0W7qJ_mm1-J7L/pub?gid=1647956932&single=true&output=tsv
    // ru - https://docs.google.com/spreadsheets/d/e/2PACX-1vQaEFU97vyhfSgaz-BhdN6sFIZvO-IR5dMbHOzF5lQt80oqCMKfLdZxQ0hcy7AdaMk0W7qJ_mm1-J7L/pub?gid=1865856919&single=true&output=tsv
    
    [CreateAssetMenu(menuName = "Defs/LocaleDef", fileName = "LocaleDef")]
    public class LocaleDef : ScriptableObject
    {
        [SerializeField] private string _url;
        [SerializeField] private List<LocaleItem> _localeItems;

        private UnityWebRequest _request;

        public Dictionary<string, string> GetData()
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var localeItem in _localeItems)
            {
                dictionary.Add(localeItem.Key, localeItem.Value);
            }

            return dictionary;
        }

        [ContextMenu("Update locale")]
        public void UpdateLocale()
        {
            if (_request != null) return;

            _request = UnityWebRequest.Get(_url);
            _request.SendWebRequest().completed += OnDataLoaded;
        }

        private void OnDataLoaded(AsyncOperation operation)
        {
            if (operation.isDone)
            {
                var rows = _request.downloadHandler.text.Split('\n');
                _localeItems.Clear();
                foreach (var row in rows)
                {
                    AddLocaleItem(row);
                }
            }
        }

        private void AddLocaleItem(string row)
        {
            try
            {
                var parts = row.Split('\t');
                _localeItems.Add(new LocaleItem {Key = parts[0], Value = parts[1]});
            }
            catch (Exception e)
            {
                Debug.LogError($"Can't parse row: {row}.\n {e}");
            }
        }

        [Serializable]
        private class LocaleItem
        {
            public string Key;
            public string Value;
        }
    }
}