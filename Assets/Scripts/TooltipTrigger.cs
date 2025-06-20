using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string kpi;
    [TextArea]
    public string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var dashboard = FindFirstObjectByType<UIDashboard>();
        if (dashboard != null)
        {
            dashboard.ShowTooltip(kpi, tooltipMessage);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var dashboard = FindFirstObjectByType<UIDashboard>();
        if (dashboard != null)
        {
            dashboard.ClearTooltips();
        }
    }
} 