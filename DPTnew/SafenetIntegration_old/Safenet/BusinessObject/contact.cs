#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    contact.cs
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
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class contact
  {
    #region private properties
    private bool? _enabled;
    private bool? _default;
    #endregion

    #region public properties
    public string emailId { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string middleName { get; set; }
    public string locale { get; set; }
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
    public bool @default
    {
      get
      {
        if (this._default.HasValue)
        {
          return this._default.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._default = value;
      }
    }
    [XmlIgnore()]
    public bool defaultSpecified
    {
      get
      {
        return this._default.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._default = null;
        }
      }
    }
    #endregion
  }
}
