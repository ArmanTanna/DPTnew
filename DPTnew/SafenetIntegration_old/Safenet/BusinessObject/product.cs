#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    product.cs
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
  public class product
  {
    #region private properties
    private int? _productIdentifier;
    private int? _enforcementId;
    private productRehost? _rehost;
    private bool? _upgradeToDriverless;
    private bool? _cloneProtection;
    private bool? _enabled;
    private bool? _deployed;
    #endregion

    public product()
    {
      this.memorySegment = new List<memorySegment>();
      this.productLicensingAttribute = new List<productLicensingAttribute>();
      this.productFeatureRef = new List<productFeatureRef>();
    }
    #region public properties
    public string productName { get; set; }
    public int baseProductId { get; set; }
    public int productIdentifier
    {
      get
      {
        if (this._productIdentifier.HasValue)
        {
          return this._productIdentifier.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._productIdentifier = value;
      }
    }
    [XmlIgnore()]
    public bool productIdentifierSpecified
    {
      get
      {
        return this._productIdentifier.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._productIdentifier = null;
        }
      }
    }
    public int enforcementId
    {
      get
      {
        if (this._enforcementId.HasValue)
        {
          return this._enforcementId.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._enforcementId = value;
      }
    }
    [XmlIgnore()]
    public bool enforcementIdSpecified
    {
      get
      {
        return this._enforcementId.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._enforcementId = null;
        }
      }
    }
    public productLifeCycleStage LifeCycleStage { get; set; }
    public productEnforcementProductType EnforcementProductType { get; set; }
    public productEnforcementProtectionType EnforcementProtectionType { get; set; }
    public productRehost rehost
    {
      get
      {
        if (this._rehost.HasValue)
        {
          return this._rehost.Value;
        }
        else
        {
          return default(productRehost);
        }
      }
      set
      {
        this._rehost = value;
      }
    }
    [XmlIgnore()]
    public bool rehostSpecified
    {
      get
      {
        return this._rehost.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._rehost = null;
        }
      }
    }
    public bool upgradeToDriverless
    {
      get
      {
        if (this._upgradeToDriverless.HasValue)
        {
          return this._upgradeToDriverless.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._upgradeToDriverless = value;
      }
    }
    [XmlIgnore()]
    public bool upgradeToDriverlessSpecified
    {
      get
      {
        return this._upgradeToDriverless.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._upgradeToDriverless = null;
        }
      }
    }
    public bool cloneProtection
    {
      get
      {
        if (this._cloneProtection.HasValue)
        {
          return this._cloneProtection.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._cloneProtection = value;
      }
    }
    [XmlIgnore()]
    public bool cloneProtectionSpecified
    {
      get
      {
        return this._cloneProtection.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._cloneProtection = null;
        }
      }
    }
    public string productDescription { get; set; }
    public string refId1 { get; set; }
    public string refId2 { get; set; }
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
    public bool deployed
    {
      get
      {
        if (this._deployed.HasValue)
        {
          return this._deployed.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._deployed = value;
      }
    }
    [XmlIgnore()]
    public bool deployedSpecified
    {
      get
      {
        return this._deployed.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._deployed = null;
        }
      }
    }
    public string version { get; set; }
    [XmlElement("productFeatureRef")]
    public List<productFeatureRef> productFeatureRef { get; set; }
    [XmlElement("productLicensingAttribute")]
    public List<productLicensingAttribute> productLicensingAttribute { get; set; }
    [XmlElement("memorySegment")]
    public List<memorySegment> memorySegment { get; set; }
    #endregion
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productLifeCycleStage
  {

    /// <remarks/>
    DRAFT,

    /// <remarks/>
    COMMIT,

    /// <remarks/>
    EOL,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productEnforcementProductType
  {

    /// <remarks/>
    BASE,

    /// <remarks/>
    PROVISIONAL,

    /// <remarks/>
    MODIFICATION,

    /// <remarks/>
    CANCELLATION,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productEnforcementProtectionType
  {

    /// <remarks/>
    HL,

    /// <remarks/>
    SL_AdminMode,

    /// <remarks/>
    SL_UserMode,

    /// <remarks/>
    HL_or_SL_AdminMode,

    /// <remarks/>
    HL_or_SL_AdminMode_or_SL_UserMode,

    /// <remarks/>
    Cloud,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productRehost
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    ENABLE,

    /// <remarks/>
    DISABLE,

    /// <remarks/>
    LEAVE_AS_IS,

    /// <remarks/>
    SAOT,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class productFeatureRef
  {
    private productFeatureRefActionName? _actionName;

    public productFeatureRef()
    {
      this.productFeatureLicenseModel = new productFeatureLicenseModel();
    }

    public int featureId { get; set; }

    public productFeatureRefActionName actionName
    {
      get
      {
        if (this._actionName.HasValue)
        {
          return this._actionName.Value;
        }
        else
        {
          return default(productFeatureRefActionName);
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

    public productFeatureLicenseModel productFeatureLicenseModel { get; set; }

    public productFeatureRefLmStatus lmStatus { get; set; }

    public bool excludable { get; set; }

    public string featureName { get; set; }

    public int featureIdentifier { get; set; }

    public string featureDescription { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productFeatureRefActionName
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    CANCEL,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class productFeatureLicenseModel
  {

    private productFeatureLicenseModelActionName? _actionName;

    public productFeatureLicenseModel()
    {
      this.licenseModel = new licenseModel();
    }

    public productFeatureLicenseModelActionName actionName
    {
      get
      {
        if (this._actionName.HasValue)
        {
          return this._actionName.Value;
        }
        else
        {
          return default(productFeatureLicenseModelActionName);
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

    public licenseModel licenseModel { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productFeatureLicenseModelActionName
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    SET,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class memoryType
  {
    public int id { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class memorySegment
  {
    public memorySegment()
    {
      this.memoryType = new List<memoryType>();
    }

    public string name { get; set; }

    public string description { get; set; }

    public string text { get; set; }

    public int size { get; set; }

    public string offset { get; set; }

    public string color { get; set; }

    public bool saot { get; set; }

    [XmlElement("memoryType")]
    public List<memoryType> memoryType { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class productLicensingAttribute
  {

    private int? _attributeId;

    private int? _associatedAttributeMaster;

    private int? _sameAttributeMaster;

    public int attributeId
    {
      get
      {
        if (this._attributeId.HasValue)
        {
          return this._attributeId.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._attributeId = value;
      }
    }

    [XmlIgnore()]
    public bool attributeIdSpecified
    {
      get
      {
        return this._attributeId.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._attributeId = null;
        }
      }
    }

    public string attrValue { get; set; }

    public int isvPermission { get; set; }

    public int endUserPermission { get; set; }

    public int associatedAttributeMaster
    {
      get
      {
        if (this._associatedAttributeMaster.HasValue)
        {
          return this._associatedAttributeMaster.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._associatedAttributeMaster = value;
      }
    }

    [XmlIgnore()]
    public bool associatedAttributeMasterSpecified
    {
      get
      {
        return this._associatedAttributeMaster.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._associatedAttributeMaster = null;
        }
      }
    }

    public int sameAttributeMaster
    {
      get
      {
        if (this._sameAttributeMaster.HasValue)
        {
          return this._sameAttributeMaster.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._sameAttributeMaster = value;
      }
    }

    [XmlIgnore()]
    public bool sameAttributeMasterSpecified
    {
      get
      {
        return this._sameAttributeMaster.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._sameAttributeMaster = null;
        }
      }
    }

    public bool saot { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum lmAttributeActionName
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    SET,

    /// <remarks/>
    ADD,

    /// <remarks/>
    SUB,

    /// <remarks/>
    CANCEL,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productFeatureRefLmStatus
  {

    /// <remarks/>
    DEFINED,

    /// <remarks/>
    SAOT,
  }
}
