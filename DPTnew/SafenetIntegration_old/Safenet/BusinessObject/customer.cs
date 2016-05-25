#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    customer.cs
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
  public class customer
  {
    private bool? _enabled;

    public customer()
    {
      this.defaultContact = new contact();
      this.addresses = new addresses();
    }

    public customerTypeEnum type { get; set; }

    public string name { get; set; }

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

    public string description { get; set; }

    public string crmId { get; set; }

    public string refId { get; set; }

    public string phone { get; set; }

    public string fax { get; set; }

    public string vendorId { get; set; }

    public addresses addresses { get; set; }

    public contact defaultContact { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum customerTypeEnum
  {

    /// <remarks/>
    org,

    /// <remarks/>
    ind,
  }

}

