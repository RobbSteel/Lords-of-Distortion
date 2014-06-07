using UnityEngine;
using System.Collections;

public class PlayerArrow : MonoBehaviour {

	public UILabel nameLabel;



	public void AttachToPlayer(Transform player, string name)
	{
		GetComponent<UIFollowTarget>().Target = player;
		nameLabel.text = name.ToUpper();
		nameLabel.gameObject.SetActive(true);
	}
}
