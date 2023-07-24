using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcavatorProgress : MonoBehaviour {

	public LayerMask layers;

	public float sx;
	public float sz;

	public int completion;
  public Image progressBar;
  public GameObject endingUI;
  public BGM bgm;
  private bool isFinished = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		int c = 0;
    if (isFinished)
      return;

		for (float i = 0; i < sx; i+= 0.1f * sx) {
			for (float j = 0; j < sz; j+= 0.1f * sz) {
				Vector3 p = transform.position + new Vector3 (i, 0, j);
				if (Physics.Linecast(p, p + new Vector3(0, -2, 0), layers)) {
					Debug.DrawLine (p, p + new Vector3 (0, -2, 0), Color.red);
				}
				else {
					Debug.DrawLine (p, p + new Vector3 (0, -2, 0), Color.green);
					c++;
				}
			}
		}
		completion = c;
    progressBar.fillAmount = Mathf.Clamp(completion/100.0F,0.0F,1.0F);

    if(completion >= 100.0F) {
      //end game
      isFinished = true;
      endingUI.SetActive (true);
      bgm.PlayHighScoreBGM();
      GetComponent<SceneReloader>().ReloadAfterSeconds(5.0F);
    }
	}



	void OnGUI() {
		//GUILayout.TextField ("Completion: " + completion + "%");
	}
}
