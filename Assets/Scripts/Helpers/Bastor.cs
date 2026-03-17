using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bastor
{
	/// <summary>
	/// This class has alot of helper methods that we use frequently
	/// </summary>
	public static class Helpers
	{
		/// <summary>
		/// Removes rich text tags from a string.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveTags(string text)
		{
			return "<noparse>" + text.Replace("</noparse>", "") + "</noparse>";
		}

		/// <summary>
		/// Returns a formatted string with correct color tags
		/// </summary>
		/// <param name="text"></param>
		/// <param name="hexColor"></param>
		/// <returns></returns>
		public static string ColoredText(object text, string hexColor)
		{
			return $"<color={hexColor}>" + text + "</color>";
		}

		/// <summary>
		/// An omega hacky way of converting a regular audio slider into a logarithmic one.
		/// </summary>
		/// <param name="sliderValue"></param>
		/// <returns></returns>
		public static float ConvertToLogarithmic(float sliderValue)
		{
			// Right...
			sliderValue /= -80.0f;

			// Evil magic numbers
			float b = 100 / 3.0f;
			float c = Mathf.Log(16.0f);

			// What the fuck
			return Mathf.Clamp(b * (Mathf.Exp(c * sliderValue) - 1.0f) / 500 * -80.0f, -80, 0);
		}

		/// <summary>
		/// Converts a hex string to a color.
		/// </summary>
		/// <param name="hexColor"></param>
		/// <param name="opacity"></param>
		/// <returns></returns>
		public static Color HexToColor(string hexColor, float opacity = 1.0f)
		{
			Color newColor;

			if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
			{
				newColor.a = opacity;

				return newColor;
			}

			return Color.white;
		}


		public static bool IntToBool(int value)
		{
			return value != 0;
		}

		public static int BoolToInt(bool value)
		{
			return value ? 1 : 0;
		}

		/// <summary>
		/// Murders all children from a transform.
		/// </summary>
		/// <param name="parent"></param>
		public static void KillChildren(Transform parent, int ignoreTill = 0)
		{
			int index = 0;
			foreach (Transform child in parent)
			{
				// The first few will be ignored haha xd thanks skyde for cool content panel
				if(index < ignoreTill)
				{
					index++;
					continue;
				}

				Object.Destroy(child.gameObject);
				index++;
			}
		}

		/// <summary>
		/// Gets the percentage of a value (X), between a minimum (Y) and maximum value (Z)
		/// Returns a value between 0.0f - 1.0f
		/// </summary>
		/// <param name="currentValue"> X </param>
		/// <param name="minValue"> Y </param>
		/// <param name="maxValue"> Z </param>
		public static float GetPercentageBetweenValues(float currentValue, float minValue, float maxValue)
		{
			return Mathf.Clamp((currentValue - minValue) / (maxValue - minValue), 0f, 1f);
		}
	}

	public static class RectTransformExtensions
	{
		public static void SetLeft(this RectTransform rt, float left)
		{
			rt.offsetMin = new Vector2(left, rt.offsetMin.y);
		}

		public static void SetRight(this RectTransform rt, float right)
		{
			rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
		}

		public static void SetTop(this RectTransform rt, float top)
		{
			rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
		}

		public static void SetBottom(this RectTransform rt, float bottom)
		{
			rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
		}
	}
}