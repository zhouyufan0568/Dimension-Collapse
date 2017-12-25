using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DimensionCollapse
{
	public class PlayerUI : MonoBehaviour {

		#region Public Properties

		[Tooltip("UI Slider to display Player's Health")]
		public Slider PlayerHealthSlider;

		[Tooltip("UI raw image to display Player's Direction")]
		public RawImage PlayerDirectionSignal;

		[Tooltip("UI canvas to display Player's backpack")]
		public GameObject PlayerBackpackCanvas;

		public GameObject dynamicUI;

		#endregion

		#region Private Properties

		private PlayerManager _target;

		#endregion

		#region MonoBehaviour Messages

		void Awake()
		{
			
		}

		// Use this for initialization
		void Start () {
			
		}

		// Update is called once per frame
		void Update () {

			if (_target.isAlive == false&&dynamicUI.GetActive()==true) {
				dynamicUI.SetActive (false);
			}else if(_target.isAlive == true&&GameObject.Find ("DynamicUI")==false){
				dynamicUI.SetActive (true);
			}

			// Reflect the Player Health
			if (PlayerHealthSlider != null) 
			{
				PlayerHealthSlider.value = _target.Health;
			}

			if (PlayerDirectionSignal != null) {
				PlayerDirectionSignal.GetComponent<RawImage> ().uvRect = new Rect (_target.Direction/360+0.25f, 0, 0.5f, 1);
			}

			// Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
			if (_target == null) 
			{
				Destroy(this.gameObject);
				return;
			}
		}

		#endregion

		#region Public Methods

		public void SetTarget(PlayerManager target)
		{
			if (target == null) 
			{
				Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.",this);
				return;
			}
			// Cache references for efficiency
			_target = target;
		}

		#endregion

	}

} 
