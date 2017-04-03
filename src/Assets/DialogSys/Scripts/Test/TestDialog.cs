using UnityEngine;
using System.Collections;

public class TestDialog : MonoBehaviour {
	public GameObject[] objects;

	GameObject lookAtObj;
	Vector3 camRotDefault;

	void Start() {
		camRotDefault = Camera.main.transform.localEulerAngles;

		SimpleDialogSys.Instance.DialogStep += (dialogId, step) => {
			if (dialogId == "firststeps") {
				if (step < objects.Length) {
					lookAtObj = objects[step];
				}
				if (step == 2) {
					GameObject.Find("LeftMouseButton").GetComponent<Renderer>().material.color = Color.red;
				}
				if (step == 3) {
					GameObject.Find("LeftMouseButton").GetComponent<Renderer>().material.color = Color.white;
					GameObject.Find("RightMouseButton").GetComponent<Renderer>().material.color = Color.red;
				}
			}

			Debug.Log("Dialog with id: " + dialogId + " stepped forward to index: " + step);
		};

		SimpleDialogSys.Instance.DialogEnd += (dialogId) => {
			if (dialogId == "firststeps") {
				// reset
				lookAtObj = null;
				Camera.main.transform.localEulerAngles = camRotDefault;

				GameObject.Find("LeftMouseButton").GetComponent<Renderer>().material.color = Color.white;
				GameObject.Find("RightMouseButton").GetComponent<Renderer>().material.color = Color.white;
			}

			Debug.Log("Dialog with id: " + dialogId + " has ended!");
		};

		SimpleDialogSys.Instance.TriggerDialog ("firststeps");
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Alpha1))
			SimpleDialogSys.Instance.TriggerDialog ("welcome");
		if (Input.GetKeyDown(KeyCode.Alpha2))
			SimpleDialogSys.Instance.TriggerDialog ("firststeps");

		if (lookAtObj) {
			Vector3 lookDir = lookAtObj.transform.position - Camera.main.transform.position;
			Quaternion lookRot = Quaternion.LookRotation (lookDir);
			Camera.main.transform.rotation = Quaternion.Slerp (Camera.main.transform.rotation, lookRot, Time.deltaTime * 10f);
		}
	}
}
