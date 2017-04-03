using UnityEngine;
using System.Collections;

public class StringArrayDataProvider : MonoBehaviour, IDialogDataProvider {
	public DialogItem[] dialogs;

	public DialogItem[] GetDialogItems() {
		return dialogs;
	}

	public DialogItem GetItemById(string id) {
		foreach (DialogItem di in dialogs) {
			if (di.Id == id)
				return di;
		}
		return null;
	}
}
