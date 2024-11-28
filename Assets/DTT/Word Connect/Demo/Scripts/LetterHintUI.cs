    using UnityEngine;
using UnityEngine.UI;

namespace DTT.WordConnect.Demo
{
    /// <summary>
    /// This class will update the information and disable the use of the letter hint button when necessary.
    /// </summary>
    public class LetterHintUI : StateUpdateUI
    {
        /// <summary>
        /// The textfield which will be updated with the balance.
        /// </summary>
        [SerializeField]
        private Text _textField;

        /// <summary>
        /// The button object which controls the use of a letter hint.
        /// </summary>
        [SerializeField]
        private Button _letterHintButton;

        /// <summary>
        /// Controls the alpha of the letter hint button and all children.
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// Once a game state occurs, update the balance of letter hints and disable the button if the balance is expended.
        /// </summary>
        /// <param name="state">The class containing the new game state data.</param>
        public override void UpdateUI(WordConnectState state)
        {
            // Set the textfield to the players balance of letter hints.
            _textField.text = state.LetterHintBalance.ToString();
            if (state.LetterHintBalance <= 0)
            {
                // If the player has no letter hints left, disable the button.
                _letterHintButton.interactable = false;
                _canvasGroup.alpha = 0.5f;
            }
            else
            {
                _letterHintButton.interactable = true;
                _canvasGroup.alpha = 1f;
            }
        }
    }
}