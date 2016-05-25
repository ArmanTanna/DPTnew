using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SafenetIntegration.Safenet.BusinessObject
{
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class entitlementItemActivationAttribute
  {
    private entitlementItemActivationAttributeGroupName? _groupName;

    private entitlementItemActivationAttributeIsvPermission? _isvPermission;

    private entitlementItemActivationAttributeEndUserPermission? _endUserPermission;

    public entitlementItemActivationAttributeAttributeName AttributeName { get; set; }

    public string attributeValue { get; set; }

    public entitlementItemActivationAttributeGroupName groupName
    {
      get
      {
        if (this._groupName.HasValue)
        {
          return this._groupName.Value;
        }
        else
        {
          return default(entitlementItemActivationAttributeGroupName);
        }
      }
      set
      {
        this._groupName = value;
      }
    }

    [XmlIgnore()]
    public bool groupNameSpecified
    {
      get
      {
        return this._groupName.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._groupName = null;
        }
      }
    }

    public entitlementItemActivationAttributeIsvPermission isvPermission
    {
      get
      {
        if (this._isvPermission.HasValue)
        {
          return this._isvPermission.Value;
        }
        else
        {
          return default(entitlementItemActivationAttributeIsvPermission);
        }
      }
      set
      {
        this._isvPermission = value;
      }
    }

    [XmlIgnore()]
    public bool isvPermissionSpecified
    {
      get
      {
        return this._isvPermission.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._isvPermission = null;
        }
      }
    }

    public object subGroupName { get; set; }

    public entitlementItemActivationAttributeEndUserPermission endUserPermission
    {
      get
      {
        if (this._endUserPermission.HasValue)
        {
          return this._endUserPermission.Value;
        }
        else
        {
          return default(entitlementItemActivationAttributeEndUserPermission);
        }
      }
      set
      {
        this._endUserPermission = value;
      }
    }

    [XmlIgnore()]
    public bool endUserPermissionSpecified
    {
      get
      {
        return this._endUserPermission.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._endUserPermission = null;
        }
      }
    }
  }
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemActivationAttributeAttributeName
  {

    /// <remarks/>
    ACKNOWLEDGEMENT_REQUEST,

    /// <remarks/>
    C2V,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemActivationAttributeGroupName
  {

    /// <remarks/>
    [XmlEnum("LICENSE TERMS")]
    LICENSETERMS,

    /// <remarks/>
    CONCURRENCY,

    /// <remarks/>
    ACCESSIBILITY,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemActivationAttributeIsvPermission
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    READ,

    /// <remarks/>
    WRITE,

    /// <remarks/>
    ORDER_WRITE,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemActivationAttributeEndUserPermission
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    READ,

    /// <remarks/>
    WRITE,

    /// <remarks/>
    ORDER_WRITE,
  }
}