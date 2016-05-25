#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    activationAttribute.cs
//
// Author:      Marco D'Alessandro
// Date:        July 2014
//
#endregion
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
  public class activationAttribute
  {
    private activationAttributeGroupName? _groupName;

    private activationAttributeIsvPermission? _isvPermission;

    private activationAttributeEndUserPermission? _endUserPermission;

    public activationAttributeAttributeName attributeName { get; set; }

    public string attributeValue { get; set; }

    public activationAttributeGroupName groupName
    {
      get
      {
        if (_groupName.HasValue)
        {
          return _groupName.Value;
        }
        else
        {
          return default(activationAttributeGroupName);
        }
      }
      set
      {
        _groupName = value;
      }
    }

    [XmlIgnore()]
    public bool groupNameSpecified
    {
      get
      {
        return _groupName.HasValue;
      }
      set
      {
        if (value == false)
        {
          _groupName = null;
        }
      }
    }

    public activationAttributeIsvPermission isvPermission
    {
      get
      {
        if (_isvPermission.HasValue)
        {
          return _isvPermission.Value;
        }
        else
        {
          return default(activationAttributeIsvPermission);
        }
      }
      set
      {
        _isvPermission = value;
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

    public activationAttributeEndUserPermission endUserPermission
    {
      get
      {
        if (this._endUserPermission.HasValue)
        {
          return this._endUserPermission.Value;
        }
        else
        {
          return default(activationAttributeEndUserPermission);
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
  public enum activationAttributeAttributeName
  {

    /// <remarks/>
    C2V,

    /// <remarks/>
    ProtectionKeyId,

    /// <remarks/>
    CLEAR_BEFORE_APPLYING_UPDATE,

    /// <remarks/>
    ACKNOWLEDGEMENT_REQUEST,

  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum activationAttributeGroupName
  {

    /// <remarks/>
    LICENSE_TERMS,

    /// <remarks/>
    CONCURRENCY,

    /// <remarks/>
    ACCESSIBILITY,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum activationAttributeIsvPermission
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    READ,

    /// <remarks/>
    WRITE,

    /// <remarks/>
    READ_WRITE,

    /// <remarks/>
    ORDER_WRITE,
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum activationAttributeEndUserPermission
  {

    /// <remarks/>
    NONE,

    /// <remarks/>
    READ,

    /// <remarks/>
    WRITE,

    /// <remarks/>
    READ_WRITE,

    /// <remarks/>
    ORDER_WRITE,
  }
}