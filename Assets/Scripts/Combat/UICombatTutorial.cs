using CombatSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TutorialPanelInfo
{
    public CombatTurn matchCombatTurn;
    public UIShowPanel panelToShow;
}

public class UICombatTutorial : MonoBehaviour
{
    [SerializeField] bool tutorialEnabled;

    public bool TutorialEnabled
    {
        get => tutorialEnabled;
        set
        {
            tutorialEnabled = value;
            PlayerPrefs.SetInt("CombatTutorialEnabled", value ? 1 : 0);
        }
    }

    [SerializeField] List<TutorialPanelInfo> tutorialPanels;

    private void Awake()
    {
        CombatManager.onCombatInitialized.AddListener(OnCombatInitialized);
        CombatManager.onCombatEnded.AddListener(OnCombatEnded);
        CombatManager.onTurnStep.AddListener(OnTurnStep);


        tutorialEnabled = PlayerPrefs.GetInt("CombatTutorialEnabled", 1) == 1;
    }


    private void OnCombatInitialized()
    {
        // tutorial not enabled, ignore combat callbacks
        if (!TutorialEnabled)
            return;
    }
    private void OnCombatEnded()
    {
        // tutorial not enabled, ignore combat callbacks
        if (!TutorialEnabled)
            return;

        // hide all panels
        foreach (var panel in tutorialPanels)
        {
            panel.panelToShow.Show(false);
        }

        GameManager.OrderCounter.ShowRecipeItem(true);

        // all combat sequence is completed, disable tutorial
        if (GameManager.CombatManager.Sequence.IsOver)
            TutorialEnabled = false;
    }

    TutorialPanelInfo GetMatchingTurnPanel(CombatTurn combatTurn)
    {
        return tutorialPanels.FirstOrDefault((e) =>
            e.matchCombatTurn.Equals(other: combatTurn)
        );
    }

    private void OnTurnStep(StepConfig stepConfig)
    {
        // tutorial not enabled, ignore combat callbacks
        if (!TutorialEnabled)
            return;

        GameManager.OrderCounter.ShowRecipeItem(false);

        // hide all panels
        foreach (var panel in tutorialPanels)
        {
            panel.panelToShow.Show(false);
        }

        // get matching panel
        TutorialPanelInfo tutorialPanel = GetMatchingTurnPanel(stepConfig.CurrentTurn);
        if (tutorialPanel != null)
        {
            tutorialPanel.panelToShow.Show(true);
            stepConfig.MoveToNextStep = false;
        }


    }

    public void OnTutorialPanelFinish()
    {
        GameManager.CombatManager.NextStep();
    }
}
