#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    licenseModel.cs
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
  public class licenseModel
  {
    public licenseModel()
    {
      this.lmAttribute = new List<lmAttribute>();
    }

    public int licenseModelId { get; set; }

    public string licenseModelName { get; set; }

    public int licenseType { get; set; }

    public string licenseModelDescription { get; set; }

    [XmlElement("lmAttribute")]
    public List<lmAttribute> lmAttribute { get; set; }
  }
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class lmAttribute
  {
    private lmAttributeActionName? _actionName;

    public lmAttribute()
    {
      this.attribute = new attribute();
    }

    public attribute attribute { get; set; }

    public string attributeValue { get; set; }

    public bool saot { get; set; }

    public lmAttributeActionName actionName
    {
      get
      {
        if (this._actionName.HasValue)
        {
          return this._actionName.Value;
        }
        else
        {
          return default(lmAttributeActionName);
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
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class attribute
  {
    private int? _displayOrder;

    private int? _attributeLevel;

    private int? _associatedAttributeMaster;

    private int? _sameAttributeMaster;

    private int? _sameAttributePriority;

    public attribute()
    {
      this.attributeValueChoice = new List<string>();
    }

    public int attributeId { get; set; }

    public string attributeName { get; set; }

    public int attributeType { get; set; }

    [XmlArrayItem("valueOption", IsNullable = false)]
    public List<string> attributeValueChoice { get; set; }

    public string attributeDefaultValue { get; set; }

    public int displayOrder
    {
      get
      {
        if (this._displayOrder.HasValue)
        {
          return this._displayOrder.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._displayOrder = value;
      }
    }

    [XmlIgnore()]
    public bool displayOrderSpecified
    {
      get
      {
        return this._displayOrder.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._displayOrder = null;
        }
      }
    }

    public int attributeLevel
    {
      get
      {
        if (this._attributeLevel.HasValue)
        {
          return this._attributeLevel.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._attributeLevel = value;
      }
    }

    [XmlIgnore()]
    public bool attributeLevelSpecified
    {
      get
      {
        return this._attributeLevel.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._attributeLevel = null;
        }
      }
    }

    public string associatedLockCriteria { get; set; }

    public string associatedLockCriteriaValue { get; set; }

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

    public int sameAttributePriority
    {
      get
      {
        if (this._sameAttributePriority.HasValue)
        {
          return this._sameAttributePriority.Value;
        }
        else
        {
          return default(int);
        }
      }
      set
      {
        this._sameAttributePriority = value;
      }
    }

    [XmlIgnore()]
    public bool sameAttributePrioritySpecified
    {
      get
      {
        return this._sameAttributePriority.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._sameAttributePriority = null;
        }
      }
    }

    public string parameterGroupName { get; set; }

    public string parameterSubGroupName { get; set; }

    public attributeIsvPermission isvPermission { get; set; }

    public bool nullable { get; set; }

    public bool saotAllowed { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum attributeIsvPermission
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