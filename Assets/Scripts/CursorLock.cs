using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private bool is_cursor_locked = true;

    public Component controller_component;

    void Start()
    {
        LockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && is_cursor_locked)
            UnlockCursor();

        else if (Input.GetMouseButton(0) && !is_cursor_locked)
        {
            if (MenuControll.isPaused)
                return;

            LockCursor();
        }
    }

    private void UnlockCursor()
    {
        is_cursor_locked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (controller_component != null)
            (GetComponent(controller_component.GetType()) as MonoBehaviour).enabled = false;
    }

    private void LockCursor()
    {
        is_cursor_locked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (controller_component != null)
            (GetComponent(controller_component.GetType()) as MonoBehaviour).enabled = true;
    }
}
