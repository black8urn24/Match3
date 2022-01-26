using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Utilities
{
    public class Fps : MonoBehaviour
    {
        #region Variables
        private string label = "";
		private float count;
        #endregion

        #region Unity Methods
        IEnumerator Start()
		{
			GUI.depth = 2;
			while (true)
			{
				if (Time.timeScale == 1)
				{
					yield return new WaitForSeconds(0.1f);
					count = (1 / Time.deltaTime);
					label = "FPS :" + (Mathf.Round(count));
				}
				else
				{
					label = "Pause";
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

		void OnGUI()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 45;
			GUI.Label(new Rect(10, (Screen.height - 50), 100, 100), label.ToCyan().ToBold(), style);
		}
        #endregion
    }
}