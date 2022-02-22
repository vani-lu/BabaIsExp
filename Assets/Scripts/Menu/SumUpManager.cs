using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vani.UI {
    public class SumUpManager : MonoBehaviour
    {
        public Text timeText;

        public Text bonusText;

        private int m_time;

        private int m_bonus;

        // Start is called before the first frame update
        void Start()
        {
            m_time = PlayerPrefs.GetInt("ExpTime");
            m_bonus = PlayerPrefs.GetInt("Bonus");
            timeText.text = string.Format("{0}", m_time);
            bonusText.text = string.Format("{0}", m_bonus);
        }

    }

}