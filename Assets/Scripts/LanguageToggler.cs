using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageToggler : MonoBehaviour
{
    private bool isChangingLanguage = false;

    // Call this from your Button's OnClick event. No index needed!
    public void ToggleLanguage()
    {
        if (isChangingLanguage) return;
        StartCoroutine(SwitchBetweenThaiAndEnglish());
    }

    private IEnumerator SwitchBetweenThaiAndEnglish()
    {
        isChangingLanguage = true;

        // Wait for the localization system to fully initialize
        yield return LocalizationSettings.InitializationOperation;

        // Check the current active language code
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;

        // If it's Thai, switch to English. Otherwise, switch to Thai.
        string targetLanguageCode = (currentLanguage == "th") ? "en" : "th";

        // Find the matching locale in your project's Available Locales
        Locale targetLocale = LocalizationSettings.AvailableLocales.GetLocale(targetLanguageCode);
        
        if (targetLocale != null)
        {
            LocalizationSettings.SelectedLocale = targetLocale;
        }
        else
        {
            Debug.LogWarning($"Could not find a locale for '{targetLanguageCode}'. Check your Localization Settings!");
        }

        isChangingLanguage = false;
    }
}
