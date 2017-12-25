using System;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class WeatherManager : MonoBehaviour
    {
        private static string KEY = "SkyAndWeather";

        public Material daytime;
        public Material dusk;
        public Material night;

        public GameObject HailEffect;
        public GameObject MeteorShowerEffect;

        private Material skySelected;
        private GameObject WeatherSelected;

        private enum Sky
        {
            DAYTIME,
            DUSK,
            NIGHT
        }

        private enum Weather
        {
            SUNNY,
            HAIL,
            METEORSHOWER
        }

        private static List<Tuple<Sky, Weather>> combinations;
        static WeatherManager()
        {
            combinations = new List<Tuple<Sky, Weather>>()
            {
            new Tuple<Sky, Weather>(Sky.DAYTIME, Weather.SUNNY),
            new Tuple<Sky, Weather>(Sky.DUSK, Weather.HAIL),
            new Tuple<Sky, Weather>(Sky.NIGHT, Weather.METEORSHOWER)
            };
        }

        private void Awake()
        {
            if (PhotonNetwork.isMasterClient)
            {
                SetSkyAndWeatherIndex();
            }
            LoadSkyAndWeather();
        }

        private void SetSkyAndWeatherIndex()
        {
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add(KEY, RandomSelectSkyAndWeather());
            PhotonNetwork.room.SetCustomProperties(hashtable);
        }

        private void LoadSkyAndWeather()
        {
            Tuple<Sky, Weather> combination = combinations[ObtainSkyAndWeatherIndex()];

            switch (combination.Item1)
            {
                case Sky.DAYTIME:
                    RenderSettings.skybox = daytime;
                    break;
                case Sky.DUSK:
                    RenderSettings.skybox = dusk;
                    break;
                case Sky.NIGHT:
                    RenderSettings.skybox = night;
                    break;
            }
            DynamicGI.UpdateEnvironment();
            skySelected = RenderSettings.skybox;

            GameObject weather;
            switch (combination.Item2)
            {
                case Weather.HAIL:
                    weather = HailEffect;
                    break;
                case Weather.METEORSHOWER:
                    weather = MeteorShowerEffect;
                    break;
                default:
                    weather = null;
                    break;
            }

            if (weather != null)
            {
                WeatherSelected = Instantiate(weather, Vector3.zero, Quaternion.identity);

                ParticleSystem[] particleSystems = WeatherSelected.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Play();
                }
            }
        }

        private int RandomSelectSkyAndWeather()
        {
            System.Random random = new System.Random();
            return random.Next(0, combinations.Count);
        }

        private int ObtainSkyAndWeatherIndex()
        {
            int index;
            try
            {
                index = Convert.ToInt32(PhotonNetwork.room.CustomProperties[KEY]);
            }
            catch (FormatException e)
            {
                Debug.Log(e);
                index = 0;
            }
            return index;
        }
    }
}
