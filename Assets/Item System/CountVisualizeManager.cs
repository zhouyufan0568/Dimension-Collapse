using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DimensionCollapse
{
    public class CountVisualizeManager : MonoBehaviour
    {
        public static CountVisualizeManager INSTANCE;

        public GameObject prefab;
        public List<Sprite> damageNumbers;
        public List<Sprite> healingNumbers;

        public int capacity = 200;
        private ObjectPoolManager.ObjectPool pool;

        public float offsetXFrom;
        public float offsetXTo;
        public float offsetYFrom;
        public float offsetYTo;
        public float offsetZFrom;
        public float offsetZTo;

        private void Awake()
        {
            if (INSTANCE != null)
            {
                Destroy(this);
                return;
            }
            INSTANCE = this;

            pool = new ObjectPoolManager.ObjectPool(prefab, capacity, true, gameObject);
        }

        public void ShowDamageCount(float count, Transform transform)
        {
            ShowDamageCount(Mathf.CeilToInt(count), transform);
        }

        public void ShowDamageCount(int count, Transform transform)
        {
            Vector3 pos = transform.position + RandomOffset(transform);
            Debug.Log(transform.position);
            Debug.Log(pos);
            ShowCount(count, pos, damageNumbers);
        }

        public void ShowHealingCount(float count, Transform transform)
        {
            ShowHealingCount(Mathf.CeilToInt(count), transform);
        }

        public void ShowHealingCount(int count, Transform transform)
        {
            Vector3 pos = transform.position + RandomOffset(transform);
            ShowCount(count, pos, healingNumbers);
        }

        private void ShowCount(int count, Vector3 position, List<Sprite> numbers)
        {
            if (count <= 0 || count > 9999)
            {
                return;
            }

            Vector3 viewPos = Camera.main.WorldToViewportPoint(position);
            if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1 || viewPos.z < 0 || viewPos.z > 100)
            {
                Debug.Log(viewPos);
                return;
            }

            int[] digits = new int[4];
            int factor = 1000;
            for (int i = 0; i < 4; i++)
            {
                digits[i] = count / factor;
                count %= factor;
                factor /= 10;
            }

            GameObject canvas = pool.Next(false);
            canvas.transform.position = position;
            canvas.transform.LookAt(PlayerManager.LocalPlayerMainCamera.transform.position);
            Transform uiObj = canvas.transform.GetChild(0);
            int index = 0;
            while (index < 4 && digits[index] == 0)
            {
                uiObj.GetChild(index++).gameObject.SetActive(false);
            }
            while (index < 4)
            {
                GameObject child = uiObj.GetChild(index).gameObject;
                Image digit = child.GetComponent<Image>();
                digit.sprite = numbers[digits[index]];
                child.SetActive(true);
                index++;
            }

            CanvasGroup canvasGroup = uiObj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;

            canvas.SetActive(true);

            StartCoroutine(FadeOutCoroutine(uiObj.gameObject, 1f));
        }

        private IEnumerator FadeOutCoroutine(GameObject uiObj, float lifetime)
        {
            CanvasGroup canvasGroup = uiObj.GetComponent<CanvasGroup>();
            float currentTime = 0f;
            while (currentTime <= lifetime)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / lifetime);
                currentTime += Time.deltaTime;
                yield return null;
            }
            uiObj.transform.parent.gameObject.SetActive(false);
        }

        private Vector3 RandomOffset(Transform transform)
        {
            return Random.Range(offsetXFrom, offsetXTo) * transform.right
                + Random.Range(offsetYFrom, offsetYTo) * transform.up
                + Random.Range(offsetZFrom, offsetZTo) * transform.forward;
        }

        //IEnumerator Test()
        //{
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(1f);
        //        ShowDamageCount(1234, PlayerManager.LocalPlayerInstance.transform.position);
        //        yield return new WaitForSeconds(1f);
        //        ShowHealingCount(5678, PlayerManager.LocalPlayerInstance.transform.position);
        //    }
        //}
    }
}
