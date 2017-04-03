using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SimpleDialogSys : MonoBehaviour {
	public static SimpleDialogSys Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<SimpleDialogSys> ();
			return instance;
		}
	}
	static SimpleDialogSys instance = null;

	public delegate void DialogStepHandler(string dialogueId, int step);
	public delegate void DialogEndHandler(string dialogueId);
	public event DialogEndHandler DialogEnd;
	public event DialogStepHandler DialogStep;

	public IDialogDataProvider dataProvider { get; set; }
	public Text dialogText;
	public Text nextStepIndicator;
	public float typingSpeed = 0.02f;
	public float indicatorBlinkSpeed = 0.2f;

	int currentSentenceIndex = -1;
	bool isTyping = false;
	bool isOpen = false;
	bool speedUp = false;
	DialogItem currentDialog;
	Animator animator;
	AudioSource audioSource;

	void Awake() {
		animator = GetComponent<Animator> ();
		animator.SetBool ("Open", false);

		audioSource = GetComponent<AudioSource> ();

		if (dataProvider == null)
			dataProvider = GetComponent<IDialogDataProvider> ();
	}

	void Update() {
		if (!isOpen)
			return;

		if (!isTyping && (Input.GetButtonDown ("Fire1") || Input.GetKeyDown (KeyCode.Space))) {
			StepForward ();
		}
		else if (isTyping && (Input.GetButtonDown ("Fire1") || Input.GetKeyDown (KeyCode.Space))) {
			speedUp = true;
		}
	}

	public void TriggerDialog(string dialogId) {
		currentDialog = dataProvider.GetItemById (dialogId);
		if (currentDialog != null) {
			currentSentenceIndex = -1;
			animator.SetBool ("Open", true);
			isOpen = true;
			nextStepIndicator.enabled = false;
		} else {
			Debug.LogError ("No dialog with the id '" + dialogId + "' found!");
		}
	}

	public void EndDialog() {
		dialogText.text = "";
		animator.SetBool ("Open", false);
		StopAllCoroutines ();
		isOpen = false;
		nextStepIndicator.enabled = false;
	}

	public void StepForward() {
		currentSentenceIndex++;
		if (currentSentenceIndex < currentDialog.Sentences.Length) {
			// next sentence needs to be displayed!
			StartCoroutine(TypeSentence(currentDialog.Sentences[currentSentenceIndex]));

			if (DialogStep != null)
				DialogStep(currentDialog.Id, currentSentenceIndex);

		} else {
			// dialog finished!
			EndDialog();
		}
	}

	void OnDialogOpened() {
		StepForward ();
	}

	void OnDialogClosed() {
		if (DialogEnd != null)
			DialogEnd(currentDialog.Id);
	}

	IEnumerator TypeSentence(DialogSentence sentence) {
		StopCoroutine ("AnimateNextStepIndicator");

		bool playSound = true;

		dialogText.text = "";
		nextStepIndicator.enabled = false;

		isTyping = true;

		int currChar = 0;
		string dText = "";
		bool highlighting = false;
		foreach (char c in sentence.Text.ToCharArray()) {
			if (!highlighting && c == '*' && sentence.Text.Substring (currChar + 1).IndexOf ("*") >= 0) {
				highlighting = true;
				dText += "<color=#00ff00>";

			} else if (highlighting && c == '*') {
				highlighting = false;
				dText += "</color>";

			} else {
				dText += c;

				if (highlighting) {
					dialogText.text = dText + "</color>";
				} else {
					dialogText.text = dText;
				}

				if (playSound)
					audioSource.Play();

				playSound = !playSound;	

				float typeDelay = typingSpeed * sentence.TypeSpeedModifier;

				if (!speedUp || !sentence.IsSkipable)
					yield return new WaitForSeconds(typeDelay);
			}

			currChar++;
		}

		isTyping = false;
		speedUp = false;

		StartCoroutine("AnimateNextStepIndicator");
	}

	IEnumerator AnimateNextStepIndicator() {
		while (true) {
			yield return new WaitForSeconds (indicatorBlinkSpeed);
			nextStepIndicator.enabled = true;
			yield return new WaitForSeconds (indicatorBlinkSpeed);
			nextStepIndicator.enabled = false;
		}
	}
}
