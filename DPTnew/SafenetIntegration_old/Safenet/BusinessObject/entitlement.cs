#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    entitlement.cs
//
// Author:      Marco D'Alessandro
// Date:        July 2014
//
#endregion
using System;
using System.Collections.Generic;
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
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class entitlement
  {

    private DateTime? _startDate;

    private DateTime? _endDate;

    private DateTime? _createDate;

    private DateTime? _commitDate;

    private bool? _enabled;

    private bool? _isTest;

    public entitlement()
    {
      this.entitlementItem = new List<entitlementItem>();
      this.policy = new entitlementPolicy();
      this.identity = new entitlementIdentity();
      this.entState = entitlementEntState.DRAFT;
      this.action = entitlementAction.SAVE;
    }

    [XmlElement(DataType = "date")]
    public System.DateTime startDate
    {
      get
      {
        if (this._startDate.HasValue)
        {
          return this._startDate.Value;
        }
        else
        {
          return default(System.DateTime);
        }
      }
      set
      {
        this._startDate = value;
      }
    }

    [XmlIgnore()]
    public bool startDateSpecified
    {
      get
      {
        return this._startDate.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._startDate = null;
        }
      }
    }

    [XmlElement(DataType = "date")]
    public System.DateTime endDate
    {
      get
      {
        if (this._endDate.HasValue)
        {
          return this._endDate.Value;
        }
        else
        {
          return default(System.DateTime);
        }
      }
      set
      {
        this._endDate = value;
      }
    }

    [XmlIgnore()]
    public bool endDateSpecified
    {
      get
      {
        return this._endDate.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._endDate = null;
        }
      }
    }

    public entitlementIdentity identity { get; set; }

    [XmlElement(DataType = "date")]
    public System.DateTime createDate
    {
      get
      {
        if (this._createDate.HasValue)
        {
          return this._createDate.Value;
        }
        else
        {
          return default(System.DateTime);
        }
      }
      set
      {
        this._createDate = value;
      }
    }

    [XmlIgnore()]
    public bool createDateSpecified
    {
      get
      {
        return this._createDate.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._createDate = null;
        }
      }
    }

    [XmlElement(DataType = "date")]
    public System.DateTime commitDate
    {
      get
      {
        if (this._commitDate.HasValue)
        {
          return this._commitDate.Value;
        }
        else
        {
          return default(System.DateTime);
        }
      }
      set
      {
        this._commitDate = value;
      }
    }

    [XmlIgnore()]
    public bool commitDateSpecified
    {
      get
      {
        return this._commitDate.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._commitDate = null;
        }
      }
    }

    public bool enabled
    {
      get
      {
        if (this._enabled.HasValue)
        {
          return this._enabled.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._enabled = value;
      }
    }

    [XmlIgnore()]
    public bool enabledSpecified
    {
      get
      {
        return this._enabled.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._enabled = null;
        }
      }
    }

    [DefaultValue(entitlementEntState.DRAFT)]
    public entitlementEntState entState { get; set; }

    [DefaultValue(entitlementAction.SAVE)]
    public entitlementAction action { get; set; }

    public bool isTest
    {
      get
      {
        if (this._isTest.HasValue)
        {
          return this._isTest.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._isTest = value;
      }
    }

    [XmlIgnore()]
    public bool isTestSpecified
    {
      get
      {
        return this._isTest.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._isTest = null;
        }
      }
    }

    [XmlElement(DataType = "integer")]
    public string customerId { get; set; }

    public string customerEmail { get; set; }

    public entitlementPolicy policy { get; set; }

    public string refId1 { get; set; }

    public string refId2 { get; set; }

    public string description { get; set; }

    [XmlElement(DataType = "integer")]
    public string partnerId { get; set; }

    public string partnerEmail { get; set; }

    [XmlElement("entitlementItem", IsNullable = true)]
    public List<entitlementItem> entitlementItem { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class entitlementIdentity
  {
    private entitlementIdentityActionName? _actionName;

    [XmlElement(DataType = "integer")]
    public string Count { get; set; }

    public entitlementIdentityActionName actionName
    {
      get
      {
        if (this._actionName.HasValue)
        {
          return this._actionName.Value;
        }
        else
        {
          return default(entitlementIdentityActionName);
        }
      }
      set
      {
        this._actionName = value;
      }
    }

    [XmlIgnore()]
    public bool actionNameSpecified
    {
      get
      {
        return this._actionName.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._actionName = null;
        }
      }
    }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementIdentityActionName
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    SET,

    /// <remarks/>
    ADD,

    /// <remarks/>
    SUB,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementEntState
  {

    /// <remarks/>
    DRAFT,

    /// <remarks/>
    COMMITTED,

    /// <remarks/>
    PRODUCT_KEY_GENERATED,

    /// <remarks/>
    PRODUCED,

    /// <remarks/>
    COMPLETED,

    /// <remarks/>
    ACKNOWLEDGED,

    /// <remarks/>
    EOL,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementAction
  {

    /// <remarks/>
    SAVE,

    /// <remarks/>
    COMMIT,

    /// <remarks/>
    PRODUCE,

    /// <remarks/>
    ENABLE,

    /// <remarks/>
    DISABLE,

    /// <remarks/>
    REOPEN,

    /// <remarks/>
    REVOKE,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class entitlementPolicy
  {
    public entitlementPolicy()
    {
      this.registrationRequired = entitlementPolicyRegistrationRequired.NOT_REQUIRED;
    }

    [DefaultValue(entitlementPolicyRegistrationRequired.NOT_REQUIRED)]
    public entitlementPolicyRegistrationRequired registrationRequired { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementPolicyRegistrationRequired
  {

    /// <remarks/>
    NOT_REQUIRED,

    /// <remarks/>
    DESIRED,

    /// <remarks/>
    MANDATORY,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class entitlementItem
  {

    private DateTime? _startDate;

    private DateTime? _endDate;

    private entitlementItemPriority? _priority;

    private entitlementItemDeploymentType? _deploymentType;

    private entitlementItemFeatureCachingMode? _featureCachingMode;

    public entitlementItem()
    {
      this.productKeyId = new List<string>();
      this.protectionKeyId = new List<string>();
      this.activationAttribute = new List<activationAttribute>();
      this.itemProduct = new List<itemProduct>();
      this.featureCachingModeValue = new entitlementItemFeatureCachingModeValue();
    }

    [XmlElement(DataType = "date")]
    public DateTime startDate
    {
      get
      {
        if (this._startDate.HasValue)
        {
          return this._startDate.Value;
        }
        else
        {
          return default(System.DateTime);
        }
      }
      set
      {
        this._startDate = value;
      }
    }

    [XmlIgnore()]
    public bool startDateSpecified
    {
      get
      {
        return this._startDate.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._startDate = null;
        }
      }
    }

    [XmlElement(DataType = "date")]
    public System.DateTime endDate
    {
      get
      {
        if (this._endDate.HasValue)
        {
          return this._endDate.Value;
        }
        else
        {
          return default(System.DateTime);
        }
      }
      set
      {
        this._endDate = value;
      }
    }

    [XmlIgnore()]
    public bool endDateSpecified
    {
      get
      {
        return this._endDate.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._endDate = null;
        }
      }
    }

    [XmlElement(DataType = "integer")]
    public string numProductKeys { get; set; }

    [XmlElement(DataType = "integer")]
    public string numActivationPerProductKey { get; set; }

    public entitlementItemPriority priority
    {
      get
      {
        if (this._priority.HasValue)
        {
          return this._priority.Value;
        }
        else
        {
          return default(entitlementItemPriority);
        }
      }
      set
      {
        this._priority = value;
      }
    }

    [XmlIgnore()]
    public bool prioritySpecified
    {
      get
      {
        return this._priority.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._priority = null;
        }
      }
    }

    [XmlElement(DataType = "integer")]
    public string vendorId { get; set; }

    [XmlElement(DataType = "integer")]
    public string enforcementId { get; set; }

    [XmlElement(DataType = "integer")]
    public string identityCount { get; set; }

    public entitlementItemLineItemType lineItemType { get; set; }

    public entitlementItemDeploymentType deploymentType
    {
      get
      {
        if (this._deploymentType.HasValue)
        {
          return this._deploymentType.Value;
        }
        else
        {
          return default(entitlementItemDeploymentType);
        }
      }
      set
      {
        this._deploymentType = value;
      }
    }

    [XmlIgnore()]
    public bool deploymentTypeSpecified
    {
      get
      {
        return this._deploymentType.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._deploymentType = null;
        }
      }
    }

    public entitlementItemFeatureCachingMode featureCachingMode
    {
      get
      {
        if (this._featureCachingMode.HasValue)
        {
          return this._featureCachingMode.Value;
        }
        else
        {
          return default(entitlementItemFeatureCachingMode);
        }
      }
      set
      {
        this._featureCachingMode = value;
      }
    }

    [XmlIgnore()]
    public bool featureCachingModeSpecified
    {
      get
      {
        return this._featureCachingMode.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._featureCachingMode = null;
        }
      }
    }

    public entitlementItemFeatureCachingModeValue featureCachingModeValue { get; set; }

    [XmlElement("itemProduct")]
    public List<itemProduct> itemProduct { get; set; }

    [XmlElement("activationAttribute")]
    public List<activationAttribute> activationAttribute { get; set; }

    [XmlElement("protectionKeyId")]
    public List<string> protectionKeyId { get; set; }

    [XmlElement("productKeyId")]
    public List<string> productKeyId { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemPriority
  {

    /// <remarks/>
    A,

    /// <remarks/>
    B,

    /// <remarks/>
    C,

    /// <remarks/>
    D,

    /// <remarks/>
    E,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemLineItemType
  {

    /// <remarks/>
    Hardware_Key,

    /// <remarks/>
    Product_Key,

    /// <remarks/>
    ProtectionKey_Update,

    /// <remarks/>
    Cloud_Named,

    /// <remarks/>
    Cloud_UnNamed,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemDeploymentType
  {

    /// <remarks/>
    Cloud_OnPremise,

    /// <remarks/>
    Cloud,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemFeatureCachingMode
  {

    /// <remarks/>
    Feature_Level,

    /// <remarks/>
    Entitlement_Level,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class entitlementItemFeatureCachingModeValue
  {

    private string _cachingModeName;

    private string _cachingModeValue;

    private entitlementItemFeatureCachingModeValueActionName? _actionName;

    public string cachingModeName
    {
      get
      {
        return this._cachingModeName;
      }
      set
      {
        this._cachingModeName = value;
      }
    }

    [XmlElement(DataType = "integer")]
    public string cachingModeValue
    {
      get
      {
        return this._cachingModeValue;
      }
      set
      {
        this._cachingModeValue = value;
      }
    }

    public entitlementItemFeatureCachingModeValueActionName actionName
    {
      get
      {
        if (this._actionName.HasValue)
        {
          return this._actionName.Value;
        }
        else
        {
          return default(entitlementItemFeatureCachingModeValueActionName);
        }
      }
      set
      {
        this._actionName = value;
      }
    }

    [XmlIgnore()]
    public bool actionNameSpecified
    {
      get
      {
        return this._actionName.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._actionName = null;
        }
      }
    }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum entitlementItemFeatureCachingModeValueActionName
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    SET,

    /// <remarks/>
    ADD,

    /// <remarks/>
    SUB,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class itemProduct
  {
    public itemProduct()
    {
      this.product = new product();
    }

    [XmlElement(DataType = "integer")]
    public string productId { get; set; }

    public product product { get; set; }

    public string productStatus { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlRoot("attribute", Namespace = "", IsNullable = false)]
  public class attributeType
  {
  }
}
