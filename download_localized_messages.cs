// you can use it to download localized messages from internet inside your game and show it to the players

public void GetIntroductoryText(Action<string> callback)
{
	string url;
	string fileName;

#if CONSOLE
	url = "https://www.example.com/console_json_file";
	fileName = "ConsoleIntroductoryText.json";
#else
	url = "https://www.example.com/pc_json_file";
	fileName = "IntroductoryText.json";
#endif
	string path = Path.Combine(Application.streamingAssetsPath, fileName);
	if (!File.Exists(path))
	{
		string error = $"Error: File {fileName} not found on the path {path}";
		Debug.LogError(error);
		path = string.Empty;
	}

	MainThreadDispatcher.StartCoroutine(DownloadText(url, callback, path, fileName));
}

// From the internet, which will also overwrite the local file, and if there is no internet, it should pull the local file.
private IEnumerator DownloadText(string url, Action<string> callback, string localFilePath, string fileName)
{
	using UnityWebRequest www = UnityWebRequest.Get(url);

	string localLocalizedText = GetIntroductoryLocalFileText(fileName);
	callback?.Invoke(localLocalizedText);
	yield return www.SendWebRequest();

	if (www.result == UnityWebRequest.Result.Success)
	{
		string jsonText = www.downloadHandler.text;
		try
		{
			File.WriteAllText(localFilePath, jsonText);
		}
		catch (Exception e)
		{
			Debug.LogError("JSON not saved in : " + localFilePath);
		}

		try
		{
			LocalizedStringData introductoryText = JsonConvert.DeserializeObject<LocalizedStringData>(jsonText);
			string localizedText = introductoryText?.GetText(LocalizationManager.Instance.CurrentLocale);
			callback?.Invoke(localizedText);
		}
		catch (Exception e)
		{
			Debug.LogError("Error deserializing introductory text: " + e.Message);
			callback?.Invoke(string.Empty);
		}
	}
	else
	{
		string localizedText = GetIntroductoryLocalFileText(fileName);
		Debug.LogError("Error downloading introductory text: " + www.error);
		callback?.Invoke(localizedText);
	}
}

private string GetIntroductoryLocalFileText(string fileName) // From local file
{
	string path = Path.Combine(Application.streamingAssetsPath, fileName);
	if (!File.Exists(path))
	{
		string error = $"Error: File {fileName} not found on the path {path}";
		Debug.LogError(error);
		return error;
	}

	try
	{
		string file = File.ReadAllText(path);
		var introductoryText = JsonConvert.DeserializeObject<LocalizedStringData>(file);
		if (introductoryText == null)
		{
			string error = $"Error: File {fileName} is bad on the path {path}";
			Debug.LogError(error);
			return error;
		}

		return introductoryText.GetText(LocalizationManager.Instance.CurrentLocale);
	}
	catch (Exception e)
	{
		System.Console.WriteLine(e);
		return e.Message;
	}
}