using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PersistenceManager {

	public static void saveData(string filePath, System.Object data) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream stream = File.Create(filePath);
		bf.Serialize(stream, data);
		stream.Close();
	}

	public static System.Object loadData(string filePath) {
		System.Object obj = null;
		if(File.Exists(filePath)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream stream = File.Open(filePath, FileMode.Open);
			obj = bf.Deserialize(stream);
			stream.Close();
		}
		return obj;
	}

	public static string readTextResource(string filePath) {
		return ((TextAsset) Resources.Load(filePath, typeof(TextAsset))).text;
	}

	public static AudioClip loadAudioClip(string fileName) {
		return (AudioClip) Resources.Load(fileName, typeof(AudioClip));
	}
}
