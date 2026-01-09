using System;
using System.Collections.Generic;
using System.Diagnostics;
using GDGame.Scripts.Events.Channels;
using Microsoft.VisualBasic.FileIO;

namespace GDGame.Scripts.Systems
{
    public enum LanguageOption { English, Czech, Ukranian }

    /// <summary>
    /// Controls the localisation for the game, loading the data from CSV files into dictionaries for each language.
    /// Uses <see cref="LanguageOption"/> to set the current language of the game. 
    /// Uses <see cref="LanguageOption.English"/> for the keys and as a fallback.
    /// </summary>
    public class LocalisationController
    {
        #region Fields
        private static LocalisationController _instance;
        private LanguageOption _currentLanguage;
        private readonly Dictionary<string, string> _englishDict;
        private readonly Dictionary<string, string> _ukranianDict;
        private readonly Dictionary<string, string> _czechDict;
        #endregion

        #region Constrcutors
        public LocalisationController()
        {
            _englishDict = LoadCSV(AppData.ENGLISH_CSV_PATH);
            _czechDict = LoadCSV(AppData.CZECH_CSV_PATH);
            _ukranianDict = LoadCSV(AppData.UKRANIAN_CSV_PATH);
            _currentLanguage = LanguageOption.English;
            EventChannelManager.Instance.InputEvents.OnLanguageSwap.Subscribe(HandleLanguageSwap);
        }
        #endregion

        #region Accessors
        public static LocalisationController Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Ensure you call Initialise() first");

                return _instance;
            }
        }
        #endregion

        #region Methods
        public static void Initialise()
        {
            if (_instance != null) return;

            _instance = new LocalisationController();
        }

        /// <summary>
        /// Creates a dictionary containing a key in English and the corresponding translation
        /// </summary>
        /// <param name="filePath">Path to the CSV File to convert</param>
        /// <returns>Game ready Language Dictionary</returns>
        private static Dictionary<string, string> LoadCSV(string filePath)
        {
            var dict = new Dictionary<string, string>();
            var parser = new TextFieldParser(filePath); 

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                dict.Add(fields[0], fields[1]);
            }
            return dict;
        }

        /// <summary>
        /// Attempt to get a string to display based on the current language setting.
        /// Uses English as a fallback option.
        /// Returns "ERROR: {key} NOT FOUND" if null
        /// </summary>
        /// <param name="key">Key to Fetch</param>
        /// <returns>Translated Key</returns>
        public string Get(string key)
        {
            var activeDict = _currentLanguage switch
            {
                LanguageOption.English => _englishDict,
                LanguageOption.Czech => _czechDict,
                LanguageOption.Ukranian => _ukranianDict,
                _ => _englishDict
            };

            if(activeDict.TryGetValue(key, out var translation))
                return translation;

            if(_englishDict.TryGetValue(key, out var fallback))
                return fallback;

            return $"ERROR: {key} NOT FOUND";
        }

        /// <summary>
        /// Cycle through the languages to display when called
        /// </summary>
        private void HandleLanguageSwap()
        {
            _currentLanguage = _currentLanguage switch
            {
                LanguageOption.English => LanguageOption.Ukranian,
                LanguageOption.Ukranian => LanguageOption.Czech,
                LanguageOption.Czech => LanguageOption.English,
                _ => LanguageOption.English
            };

            Debug.WriteLine($"New Language: {_currentLanguage}");
        }

        /// <summary>
        /// Set the current language of the Localisation Controller
        /// </summary>
        /// <param name="language">:anguage to set</param>
        public void SetLanguage(LanguageOption language) => _currentLanguage = language;
        #endregion
    }
}
