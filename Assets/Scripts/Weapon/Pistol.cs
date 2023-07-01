using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public class Pistol : Firearms
    {
        protected override void Start()
        {
            switchCurrent = 1;

            base.Start();
        }

        protected override void ReloadEnd()
        {
            base.ReloadEnd();
            uiViewer.GetComponent<UIView>().UpdateAmmoSideCurrent(CurrentAmmo);
            uiViewer.GetComponent<UIView>().UpdateAmmoSideLeaft(CurrentMaxAmmoCarried);
        }

        protected override void Shooting()
        {
            base.Shooting();
        }

        protected override void SwitchWepong()
        {
            switchNext = 0;
            animator.SetTrigger("end");
        }
    }
}