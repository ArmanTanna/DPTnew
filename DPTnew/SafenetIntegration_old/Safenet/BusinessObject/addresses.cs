#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    addresses.cs
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
  public class addresses
  {

    private bool? _shippingSameAsBilling;

    public addresses()
    {
      this.address = new List<address>();
    }

    public bool shippingSameAsBilling
    {
      get
      {
        if (this._shippingSameAsBilling.HasValue)
        {
          return this._shippingSameAsBilling.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._shippingSameAsBilling = value;
      }
    }

    [XmlIgnore()]
    public bool shippingSameAsBillingSpecified
    {
      get
      {
        return this._shippingSameAsBilling.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._shippingSameAsBilling = null;
        }
      }
    }

    [XmlElement("address")]
    public List<address> address { get; set; }
  }
}
