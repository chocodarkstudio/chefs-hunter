using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameUtils
{
    /// <summary>
    /// GameUtils for GameObject, Maths, etc </summary>
    public static class GUtils
    {
        /// <summary>
        /// One percent Viewport Height </summary>
        public static float VH => Screen.height * 0.1f;
        /// <summary>
        /// One percent Viewport Width </summary>
        public static float VW => Screen.width * 0.1f;

        #region Miscellaneous

        /// <summary>
        /// Always be sure to clean before use this </summary>
        public static Collider[] collidersAlloc = new Collider[50];

        public static void CleanCollidersAlloc() => Array.Clear(collidersAlloc, 0, collidersAlloc.Length);

        /// <summary>
        /// Always be sure to clean before use this </summary>
        public static RaycastHit[] raycastHitAlloc = new RaycastHit[50];

        public static void CleanRaycastHitAlloc() => Array.Clear(raycastHitAlloc, 0, raycastHitAlloc.Length);

        /// <summary>
        /// Transforms the fileName in streaming asset folder path </summary>
        public static string GetPathFromStreaming(string fileName) => System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

        public static bool GetEnumByStr<T>(string value, out T result)
        {
            result = default;

            if (string.IsNullOrEmpty(value))
                return false;

            bool parsed = Enum.TryParse(typeof(T), value, true, out object obj);
            if (!parsed)
                return false;

            result = (T)obj;
            return true;
        }


        /// <summary>
        /// Transforms the fileName in streaming asset folder path </summary>
        public static string GetFromStreamingAsset(string fileName) => System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

        /// <summary>
        /// Truncate the text with a newline ('\n') when a line reaches the maximum length. (only cut in spaces) </summary>
        public static string TruncateText(string text, int threshold = 40)
        {
            // If the text doesn't exceed the maximum length, return it as is
            if (text.Length < threshold)
                return text;

            // Procedure:
            // traverses line by line.
            // checks if the line exceeds the limit.
            // - if so: cuts it and adds the rest to the next line.
            // repeats the process.
            //
            // only cuts at spaces and not within words.
            // new lines can be added while iterating in the same loop.
            // lines in orignial text cannot be added with carry

            // Split the text into lines
            IEnumerator lines = text.Split("\n").GetEnumerator();

            StringBuilder truncatedText = new();
            StringBuilder carry = new();
            string line;

            // Set the maximum number of iterations to the number of characters in the text
            // to avoid an infinite loop (the loop may end before reaching the maximum)
            for (int l = 0; l < text.Length; l++)
            {
                // start with the carry and clear it
                line = carry.ToString();
                carry.Clear();

                // If there is no carry, put line
                if (line.Length == 0)
                {
                    // add line text
                    if (lines.MoveNext())
                        line += lines.Current;
                    // If there is no content, end here
                    else break;
                }


                if (line.Length >= threshold)
                {
                    // Start cutting in threshold /1.5f (Extra)
                    // Search for spaces to cut, ensuring words are not cut
                    int cutIndex = line.IndexOf(' ', (int)(threshold / 1.5f));

                    // if space was found, remove it
                    if (cutIndex > 0)
                        line = line.Remove(cutIndex, 1);
                    else
                        cutIndex = line.Length;
                    /* ONLY CUT SPACES
                    else // If no space, cut in the middle anyway
                        cutIndex = (int)(threshold / 1.5f);
                    */

                    // Put the right-half of the line in carry
                    carry.Append(line[cutIndex..]);

                    // take left-half
                    line = line[..cutIndex];
                }

                // Collect all truncated lines
                truncatedText.AppendLine(line.Trim(' '));
            }

            // Remove the starting and ending newlines and return the result
            return truncatedText.ToString().Trim('\n');
        }

        /// <summary>
        /// Get a Sprite code of Text Mesh Pro with a given sprite name </summary>
        public static string GetTMProSprite(string spriteName) => $"<sprite name='{spriteName}'>";


        /// <summary>
        /// Open a url with a string parameters </summary>
        public static void OpenUrl(string baseUrl, params string[] parameters)
        {

            // Example:
            // GUtils.OpenUrl("https://twitter.com/intent/tweet?", "text=This is my text :) \n using GUtils to open a encoded url in code");

            string endocedUrl = EncodeUrl(string.Join("&", parameters));
            Application.OpenURL(baseUrl + endocedUrl);
        }

        /// <summary>
        /// Encode each character in the URL to HtmlUrlCode </summary>
        public static string EncodeUrl(string url)
        {
            StringBuilder encodedUrl = new();

            // codes
            Dictionary<char, string> htmlUrlCodes = new()
            {
                {' ', "%20"}, {'!', "%21"},
                {'"', "%22"}, {'#', "%23"},
                {'$', "%24"}, {'%', "%25"},
                {'&', "%26"}, {'\'', "%27"},
                {'(', "%28"}, {')', "%29"},
                {'*', "%2A"}, {'+', "%2B"},
                {',', "%2C"}, {'/', "%2F"},
                {':', "%3A"}, {';', "%3B"},
                {'<', "%3C"}, {'=', "%3D"},
                {'>', "%3E"}, {'?', "%3F"},
                {'@', "%40"}, {'[', "%5B"},
                {'\\', "%5C"}, {']', "%5D"},
                {'^', "%5E"}, {'`', "%60"},
                {'{', "%7B"}, {'|', "%7C"},
                {'}', "%7D"}, {'~', "%7E"},
                {'\n', "%0D" }
            };
            bool parameterNameEnded = false;

            foreach (char c in url)
            {
                // this prevent to convert the parameter
                if (!parameterNameEnded)
                {
                    encodedUrl.Append(c);

                    // 'text=', search the '=' that means the parameter name has ended
                    if (c == '=')
                        parameterNameEnded = true;

                    continue;
                }

                if (htmlUrlCodes.TryGetValue(c, out string encoded))
                    encodedUrl.Append(encoded);
                else
                    encodedUrl.Append(c);
            }

            return encodedUrl.ToString();
        }

        /// <summary>
        /// Search an index in the dropdown that starts with the string value in lower case </summary>
        /// <returns> index or -1 </returns>
        public static int GetDropdownIndexValue(Dropdown dropdown, string value)
        {
            // invalid input
            if (dropdown == null || string.IsNullOrEmpty(value))
                return -1;

            value = value.ToLower();

            // search the value
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text.ToLower().StartsWith(value))
                    return i;
            }

            return -1;
        }

        #endregion

        #region Maths

        public static float ElapsedTime(float startedTime) => Time.time - startedTime;

        public static float TruncateFloat(float number, int decimalPlaces = 2)
        {
            float multiplier = (float)Math.Pow(10, decimalPlaces);
            return (float)Math.Round(number * multiplier) / multiplier;
        }

        public static float SnapValue(float value, float size, float pos = 0) => Mathf.Round((value - pos) / size) * size + pos;


        public static Vector3 GetRandomRotationV(Vector3 axis, float degress = 360)
        {
            return Vector3.Scale(axis * degress, new(
                UnityEngine.Random.value,
                UnityEngine.Random.value,
                UnityEngine.Random.value));
        }
        public static Quaternion GetRandomRotationQ(Vector3 axis, float degress = 360)
            => Quaternion.Euler(GetRandomRotationV(axis, degress));

        public static Vector3 GetRandomPosition(float min = -10, float max = 10)
        {
            return new(
                UnityEngine.Random.Range(min, max),
                UnityEngine.Random.Range(min, max),
                UnityEngine.Random.Range(min, max));
        }

        public static Vector3 GetRandomPositionInBounds(Bounds bounds)
        {
            float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            float randomY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
            float randomZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(randomX, randomY, randomZ);
        }

        public static bool IsAnyPointNear(Vector3 point, float distance, ref List<Vector3> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (Mathf.Abs(points[i].x - point.x) < distance
                    && Mathf.Abs(points[i].y - point.y) < distance
                    && Mathf.Abs(points[i].z - point.z) < distance)
                    return true;
            }
            return false;
        }

        public static bool IsAnyPointNear(Vector3 point, float distance, ref List<Transform> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == null)
                    continue;

                if (Mathf.Abs(points[i].position.x - point.x) < distance
                    && Mathf.Abs(points[i].position.y - point.y) < distance
                    && Mathf.Abs(points[i].position.z - point.z) < distance)
                    return true;
            }
            return false;
        }

        public static T[] RemoveNulls<T>(ref T[] arr)
        {
            int nullCount = 0;
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] == null)
                    nullCount++;

            T[] newArr = new T[arr.Length - nullCount];
            int index = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                {
                    newArr[index] = arr[i];
                    index++;
                }
            }

            return newArr;
        }

        public static Vector3[] GetPoints(LineRenderer line)
        {
            Vector3[] points = new Vector3[line.positionCount];

            for (int i = 0; i < line.positionCount; i++)
            {
                points[i] = line.GetPosition(i);
            }

            return points;
        }


        public static Vector4 ToVector4(this Quaternion q) => new(q.x, q.y, q.z, q.w);
        public static Quaternion ToQuaternion(this Vector4 v) => new(v.x, v.y, v.z, v.w);

        #endregion

        #region Editor
        public static bool IsEditorSelecting(GameObject gm, bool allowMultiple = true)
        {
#if UNITY_EDITOR
            // not selecting
            if (UnityEditor.Selection.gameObjects.Length == 0)
                return false;

            // multiple selecting
            if (allowMultiple &&
                Array.IndexOf(UnityEditor.Selection.gameObjects, gm) != -1)
                return true;

            // selecting one
            else if (UnityEditor.Selection.gameObjects[0] == gm)
                return true;

#endif
            return false;
        }
        #endregion
    }

    public static class ScriptableObjectExtension
    {
        /// <summary>
        /// Creates and returns a clone of given scriptable object </summary>
        public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
        {
            if (scriptableObject == null)
            {
                // Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
                // return (T)ScriptableObject.CreateInstance(typeof(T));

                Debug.LogError($"ScriptableObject was null. Returning null object.");
                return null;
            }

            T instance = UnityEngine.Object.Instantiate(scriptableObject);
            instance.name = scriptableObject.name; // remove (Clone) from name
            return instance;
        }
    }
}
