using UnityEngine;
using System.Collections;

[System.Serializable]
public class DialogItem {
	public string Id;
	[TextArea(0, 3)]
	public DialogSentence[] Sentences;
}

[System.Serializable]
public class DialogSentence {
	public string Text;
	public float TypeSpeedModifier = 1f;
	public bool IsSkipable = true;
}
