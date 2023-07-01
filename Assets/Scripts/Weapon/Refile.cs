using StarterAssets;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace Scripts.Weapon
{
    public class Refile : Firearms
    {
        protected override void Start()
        {
            switchCurrent = 0;

            base.Start();
        }

        protected override void Shooting()
        {
            base.Shooting();
        }

        protected override void ReloadEnd()
        {
            base.ReloadEnd();
            uiViewer.GetComponent<UIView>().UpdateAmmoMainCurrent(CurrentAmmo);
            uiViewer.GetComponent<UIView>().UpdateAmmoMainLeaft(CurrentMaxAmmoCarried);
            
        }

        protected override void SwitchWepong()
        {
            switchNext = 1;
            animator.SetTrigger("end");
        }
    }
}