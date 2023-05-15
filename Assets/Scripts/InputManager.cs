using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    private Dictionary<KeyCode, UnityEvent> keyBindings = new Dictionary<KeyCode, UnityEvent>();

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputManager>();
                if (instance == null)
                {
                    GameObject inputManagerObject = new GameObject("InputManager");
                    instance = inputManagerObject.AddComponent<InputManager>();
                }
            }
            return instance;
        }
    }

    private void Update()
    {
        // Check if any of the registered key codes were pressed down this frame
        foreach (KeyValuePair<KeyCode, UnityEvent> kvp in keyBindings)
        {
            KeyCode keyCode = kvp.Key;
            UnityEvent unityEvent = kvp.Value;

            if (Input.GetKeyDown(keyCode))
            {
                TriggerKeyCodeEvent(keyCode);
            }
        }
    }

    public void BindKeyCode(KeyCode keyCode, UnityAction callback)
    {
        if (!keyBindings.ContainsKey(keyCode))
        {
            Debug.Log("Keybind set" + keyCode);
            keyBindings[keyCode] = new UnityEvent();
        }
        keyBindings[keyCode].AddListener(callback);
    }

    public void UnbindKeyCode(KeyCode keyCode, UnityAction callback)
    {
        if (keyBindings.ContainsKey(keyCode))
        {
            keyBindings[keyCode].RemoveListener(callback);
        }
    }

    public void TriggerKeyCodeEvent(KeyCode keyCode)
    {
        if (keyBindings.ContainsKey(keyCode))
        {
            Debug.Log("Keybind invoke" + keyCode);
            keyBindings[keyCode].Invoke();
        }
    }
}

/*// for abilities manager (needed to be called in update)
void OnEnable()
{
    foreach (Ability ability in abilities)
    {
        if (!string.IsNullOrEmpty(ability.keyCode.ToString()))
        {
            InputManager.Instance.BindKeyCode(ability.keyCode, () =>
            {
                if (!ability.isCd && !disableAll) // Ability is not on cd and no ability animation is active
                {
                    MethodInfo methodInfo = GetType().GetMethod(ability.abilityName);
                    if (methodInfo != null)
                        methodInfo.Invoke(this, new object[] { ability }); // Call ability method    
                    if (CheckTriggerExists(ability.abilityName))
                    {
                        weaponAnim.SetTrigger(ability.abilityName);
                    }
                    DisableAbilityUse(ability);
                }
                else if (ability.isCd) // Ability on cd
                    ApplyCooldown(ability);
            });
        }
    }
}*/