﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.View")]
    public partial class ViewImpl : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Animator animator;

        // Called by controller event.
        public void Open()
        {
            animator.SetBool("Showed", true);
        }

        // Called by controller event.
        public void Close()
        {
            animator.SetBool("Showed", false);
        }
    }
}
