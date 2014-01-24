using UnityEngine;
using System.Collections;

public class PopulateMatches : MonoBehaviour {

	public UILabel playerlabel;
	public GameObject information;
	public UILabel playertitle;
	public PSinfo infoscript;
	///public Object labeled;
	// Use this for initialization
	void Start () {
	  // playertitle = UILabel.Instantiate(playerlabel, transform.position, transform.rotation);
		//playertitle = Instantiate(playerlabel, transform.position, transform.rotation);
		information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PSinfo>();
		//playertitle.text = infoscript.playername;
	}
	
	// Update is called once per frame
	void Update () {
	

		print(infoscript.playername);

	}
}
