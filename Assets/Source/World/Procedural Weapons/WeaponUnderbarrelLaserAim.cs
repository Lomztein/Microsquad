using UnityEngine;
using System.Collections;

public class WeaponUnderbarrelLaserAim : WeaponUnderbarrelAttachment {

	public float range;
	public LineRenderer laser;
	public Transform muzzle;
	public float width;

	void FixedUpdate () {
		Ray ray = new Ray (muzzle.position, muzzle.forward);
		RaycastHit hit;

		laser.SetWidth (width, width);
		laser.SetPosition (0, muzzle.position);
		if (Physics.Raycast (ray, out hit, range)) {
			laser.SetPosition (1, hit.point);
		}else{
			laser.SetPosition (1, ray.GetPoint (range));
		}
	}
}
