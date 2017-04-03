using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;

/* 
 * XML-Data structure:
 * 
 * <Dialogs>
 *   <DialogItem>
 *     <Id></Id>
 *     <Sentences>
 *       <Sentence>Bla</Sentence>
 *       <Sentence>Bla Bla</Sentence>
 *       ...
 *     </Sentences>
 *   </DialogItem>
 *   ...
 * </Dialogs>
 */

public class XmlResourceDataProvider : MonoBehaviour, IDialogDataProvider {
	public string filepath;

	DialogItem[] dialogs;

	void Awake() {
		Load ();
	}

	public void Load() {
		TextAsset ta = Resources.Load<TextAsset> (filepath);
		if (ta == null) {
			Debug.LogError ("TextResource not found: " + filepath);
			return;
		}

		XDocument xml = XDocument.Parse (ta.text);

		var dialogItemNodes = xml.Root.Descendants("DialogItem").ToList();

		dialogs = new DialogItem[dialogItemNodes.Count];
		for (int i = 0; i < dialogItemNodes.Count; i++) {
			DialogItem dItem = new DialogItem ();
			dItem.Id = dialogItemNodes[i].Element("Id").Value;

			var sentenceNodes = dialogItemNodes[i].Element("Sentences").Elements("Sentence").ToList();
			
			dItem.Sentences = new DialogSentence[sentenceNodes.Count];
			for (int j = 0; j < sentenceNodes.Count; j++) {
				DialogSentence s = new DialogSentence ();

				// try for a CDATA element (needed for text formatting)
				try {
					XCData xcdata = (XCData)sentenceNodes[j].FirstNode;
					s.Text = xcdata.Value;
				} catch (Exception e) {
					s.Text = sentenceNodes[j].Value;
				}

				// get sentence attrs
				if (sentenceNodes[j].Attribute("typeSpeedModifier") != null)
					s.TypeSpeedModifier = Convert.ToSingle(sentenceNodes[j].Attribute("typeSpeedModifier").Value);

				dItem.Sentences[j] = s;
			}

			dialogs[i] = dItem;
		}
	}

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
