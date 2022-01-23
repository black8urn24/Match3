using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FullSerializer;

namespace Match3.Utilities
{
    public static class FileUtilities
    {
        #region Static Methods
        public static T LoadJsonFile<T>(string path) where T : class
        {
            var vSerializer = new fsSerializer();
            var vTextAsset = Resources.Load<TextAsset>(path);
            Assert.IsNotNull((vTextAsset));
            var data = fsJsonParser.Parse(vTextAsset.text);
            object deserialized = null;
            vSerializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
            return deserialized as T;
        }

        public static bool FileExists(string path)
        {
            var textAsset = Resources.Load<TextAsset>(path);
            return textAsset != null;
        }
        #endregion
    }
}