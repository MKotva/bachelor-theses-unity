using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.PopUp
{
    public class PopUpButtonController : MonoBehaviour
    {
        [SerializeField] GameObject PopUpCanvas;
        [SerializeField] GameObject BackgroundPopUp;

        public void OnClick()
        {
            PopUpCanvas.SetActive(true);
            BackgroundPopUp.SetActive(true);
        }
    }
}
