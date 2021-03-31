// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/NewInput/InputTest.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class InputTest : InputActionAssetReference
{
    public InputTest()
    {
    }
    public InputTest(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // MouseTest
        m_MouseTest = asset.GetActionMap("MouseTest");
        m_MouseTest_Rotation = m_MouseTest.GetAction("Rotation");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_MouseTest = null;
        m_MouseTest_Rotation = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // MouseTest
    private InputActionMap m_MouseTest;
    private InputAction m_MouseTest_Rotation;
    public struct MouseTestActions
    {
        private InputTest m_Wrapper;
        public MouseTestActions(InputTest wrapper) { m_Wrapper = wrapper; }
        public InputAction @Rotation { get { return m_Wrapper.m_MouseTest_Rotation; } }
        public InputActionMap Get() { return m_Wrapper.m_MouseTest; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(MouseTestActions set) { return set.Get(); }
    }
    public MouseTestActions @MouseTest
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new MouseTestActions(this);
        }
    }
    private int m_keyboardSchemeIndex = -1;
    public InputControlScheme keyboardScheme
    {
        get

        {
            if (m_keyboardSchemeIndex == -1) m_keyboardSchemeIndex = asset.GetControlSchemeIndex("keyboard");
            return asset.controlSchemes[m_keyboardSchemeIndex];
        }
    }
}
