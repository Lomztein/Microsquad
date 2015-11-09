using UnityEngine;
using System.Collections;

public class WeaponPart : MonoBehaviour {

	/* Weapon body defines the main stats, while stats on any other part
	 * is added as percentage. Should be pretty neat. The body is also
	 * the main *body* of the architecture, and will issue commands
	 * and contain references to the rest of the parts.
	 */

	public enum Type { Body, Stock, Magazine, Optics, Barrel };
	public Type type;

	public string nameMod;
	public WeaponStats stats;
	
}

[System.Serializable]
public class WeaponStats {

	public float damage = 1f;
	public float spread = 1f;
	public float speed = 1f;
	public float weight = 1f;
	public float recoil = 1f;
	public float firerate = 1f;
	public int bulletAmount = 1;

	/// <summary>
	/// Combines the parsed stats with this stats. Nothing returned, but the parsed stats are edited.
	/// </summary>
	/// <param name="other">Other stats.</param>
	public void CombineWith (WeaponStats other) {
		other.damage *= damage;
		other.spread *= spread;
		other.speed *= speed;
		other.weight += weight;
		other.recoil *= recoil;
		other.firerate /= firerate;
		other.bulletAmount *= bulletAmount;
	}

	public WeaponStats Clone () {
		WeaponStats s = new WeaponStats();
		s.damage = damage;
		s.spread = spread;
		s.speed = speed;
		s.weight = weight;
		s.recoil = recoil;
		s.firerate = firerate;
		s.bulletAmount = bulletAmount;
		return s;
	}

	public override string ToString ()
	{
		return "Damage: " + damage.ToString () + "\nSpread: " + spread.ToString ()
			+ "\nSpeed: " + speed.ToString () + "\nWeight: " + weight.ToString ()
				+ "\nRecoil: " + recoil.ToString () + "\nFirerate: " + firerate.ToString ()
				+ "\nBullets: " + bulletAmount.ToString () + "\n R/W Ratio: " + (recoil / weight).ToString ();
	}
}