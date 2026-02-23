using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CustomStyle("SignalEmitter")]
public class FloatSignalEmitter : Marker, INotification
{
    [Tooltip("The float value you want to pass (e.g., hitstop duration)")]
    public float floatValue;

    // INotification requires an ID, we just return a default one
    public PropertyName id { get { return new PropertyName(); } }
    
    
    [SerializeField] SignalAsset m_Asset;
    
    public SignalAsset asset
    {
        get { return m_Asset; }
        set { m_Asset = value; }
    }
    
    PropertyName INotification.id
    {
        get
        {
            if (m_Asset != null)
            {
                return new PropertyName(m_Asset.name);
            }
            return new PropertyName(string.Empty);
        }
    }
}
