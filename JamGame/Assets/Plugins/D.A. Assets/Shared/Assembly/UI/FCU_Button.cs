/*
The MIT License (MIT)

Copyright (c) 2014, Unity Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DA_Assets.FCU.UI
{
    [AddComponentMenu("UI/FCU_Button", 31)]
    [Obsolete("Use DAButton or FcuButton")]
    public class FCU_Button : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField] bool m_defaultEnabled;
        [SerializeField] bool m_hoverEnabled;
        [SerializeField] bool m_pressedEnabled;
        [SerializeField] bool m_selectedEnabled;
        [SerializeField] bool m_disabledEnabled;

        [SerializeField] Color m_textDefaultColor;
        [SerializeField] Color m_textHoverColor;
        [SerializeField] Color m_textPressedColor;
        [SerializeField] Color m_textSelectedColor;
        [SerializeField] Color m_textDisabledColor;

        [SerializeField] MaskableGraphic m_buttonText;

        public bool DefaultEnabled { get => m_defaultEnabled; set => m_defaultEnabled = value; }
        public bool HoverEnabled { get => m_hoverEnabled; set => m_hoverEnabled = value; }
        public bool PressedEnabled { get => m_pressedEnabled; set => m_pressedEnabled = value; }
        public bool SelectedEnabled { get => m_selectedEnabled; set => m_selectedEnabled = value; }
        public bool DisabledEnabled { get => m_disabledEnabled; set => m_disabledEnabled = value; }
        public Color TextDefaultColor { get => m_textDefaultColor; set => m_textDefaultColor = value; }
        public Color TextHoverColor { get => m_textHoverColor; set => m_textHoverColor = value; }
        public Color TextPressedColor { get => m_textPressedColor; set => m_textPressedColor = value; }
        public Color TextSelectedColor { get => m_textSelectedColor; set => m_textSelectedColor = value; }
        public Color TextDisabledColor { get => m_textDisabledColor; set => m_textDisabledColor = value; }
        public MaskableGraphic ButtonText { get => m_buttonText; set => m_buttonText = value; }

        [SerializeField, FormerlySerializedAs("onClick")] ButtonClickedEvent _onClick = new ButtonClickedEvent();
        [Serializable]
        public class ButtonClickedEvent : UnityEvent
        {

        }

        public ButtonClickedEvent onClick
        {
            get { return _onClick; }
            set { _onClick = value; }
        }
        private void Press()
        {
            if (IsActive() == false || IsInteractable() == false)
            {
                return;
            }

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            _onClick.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (IsActive() == false || IsInteractable() == false)
            {
                return;
            }

            Press();

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            float fadeTime = colors.fadeDuration;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (base.transition == Transition.SpriteSwap)
            {
                if (interactable == true && m_buttonText != null)
                {
                    if (m_pressedEnabled)
                        m_buttonText.color = m_textPressedColor;
                }
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (base.transition == Transition.SpriteSwap)
            {
                if (interactable == true && m_buttonText != null)
                {
                    if (m_defaultEnabled)
                        m_buttonText.color = m_textDefaultColor;
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (base.transition == Transition.SpriteSwap)
            {
                if (interactable == true && m_buttonText != null)
                {
                    if (m_hoverEnabled)
                        m_buttonText.color = m_textHoverColor;
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (base.transition == Transition.SpriteSwap)
            {
                if (interactable == true && m_buttonText != null)
                {
                    if (m_defaultEnabled)
                        m_buttonText.color = m_textDefaultColor;
                }
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (base.transition == Transition.SpriteSwap)
            {
                if (interactable == true && m_buttonText != null)
                {
                    if (m_selectedEnabled)
                        m_buttonText.color = m_textSelectedColor;
                }
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            if (base.transition == Transition.SpriteSwap)
            {
                if (interactable == true && m_buttonText != null)
                {
                    if (m_defaultEnabled)
                        m_buttonText.color = m_textDefaultColor;
                }
            }
        }

        public override void Select()
        {
            base.Select();
        }

        public override bool IsInteractable()
        {
            bool _interactable = base.IsInteractable();

            if (m_buttonText == null)
            {
                return _interactable;
            }

            if (_interactable)
            {
                if (m_defaultEnabled)
                    m_buttonText.color = m_textDefaultColor;
            }
            else
            {
                if (m_disabledEnabled)
                    m_buttonText.color = m_textDisabledColor;
            }

            return _interactable;
        }
    }
}