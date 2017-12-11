using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour {

	//states of map
	public enum mapStates{
		WaitforReset,
		WaitforCollapse,
		Collapse
	}

	private mapStates state;

	private MapDynamicLoading mapdynamicloading;

	//seconds of states lasting
	[HideInInspector]
	public float SecondsOfWaitReset;
	[HideInInspector]
	public float SecondsOfWaitCollapse;
	[HideInInspector]
	public float SecondsOfCollapse;

	//time of states transition
	[HideInInspector]
	public float timeOfReset;
	[HideInInspector]
	public float timeOfCollapse;
	[HideInInspector]
	public float timeOfFinishCollapse;

	private float currentFPS;

	// Use this for initialization
	void Start () {
		mapdynamicloading = transform.GetComponent<MapDynamicLoading> ();
		state = mapStates.WaitforReset;
	}

	// Update is called once per frame
	void Update () {

		currentFPS = 1/Time.deltaTime;

		switch (state) {

		case(mapStates.WaitforReset):
			{
				if (mapdynamicloading.elapsed >= timeOfReset) {
					state = mapStates.WaitforCollapse;
					SecondsOfWaitReset *=mapdynamicloading.shrinkRatio;
					timeOfReset=timeOfFinishCollapse+SecondsOfWaitReset;
				}
				break;
			}
		case(mapStates.WaitforCollapse):
			{
				if (mapdynamicloading.elapsed >= timeOfCollapse) {
					state = mapStates.Collapse;
					SecondsOfWaitCollapse *=mapdynamicloading.shrinkRatio;
					timeOfCollapse = timeOfReset + SecondsOfWaitCollapse;
				}
				break;
			}
		case(mapStates.Collapse):
			{
				if (mapdynamicloading.elapsed >= timeOfFinishCollapse) {
					state = mapStates.WaitforReset;
					SecondsOfCollapse*=mapdynamicloading.shrinkRatio;
					timeOfFinishCollapse = timeOfCollapse + SecondsOfCollapse;
				}
				break;
			}
		}
	}

	private void OnGUI()
	{
		GUILayout.Label("FPS:" + currentFPS.ToString("f2"));
		GUILayout.Label("当前时间："+mapdynamicloading.elapsed);
		switch (state) {
		case(mapStates.WaitforReset):
			{
				GUILayout.Label(timeOfReset+"s重置安全区");
				break;
			}
		case(mapStates.WaitforCollapse):
			{
				GUILayout.Label(timeOfCollapse+"s开始塌陷");
				break;
			}
		case(mapStates.Collapse):
			{
				GUILayout.Label(timeOfFinishCollapse+"s停止塌陷");
				break;
			}
		}
	}

}
