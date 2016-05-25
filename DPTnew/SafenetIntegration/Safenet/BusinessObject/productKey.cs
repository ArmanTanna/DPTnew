#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    productKey.cs
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
  public class productKey
  {
    public productKey()
    {
      this.protectionKeyId = new List<productKeyProtectionKeyId>();
      this.activationAttribute = new List<activationAttribute>();
      this.aid = new List<productKeyAid>();
      this.productInfo = new List<productKeyProductInfo>();
      this.registrationRequired = productKeyRegistrationRequired.NOT_REQUIRED;
    }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string productKeyId { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
    public string totalEntitled { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
    public string available { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool enabled { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public object type { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified), DefaultValue(productKeyRegistrationRequired.NOT_REQUIRED)]
    public productKeyRegistrationRequired registrationRequired { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string customerEmail { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string customerName { get; set; }

    [XmlElement("productInfo", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public List<productKeyProductInfo> productInfo { get; set; }

    [XmlElement("AID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public List<productKeyAid> aid { get; set; }

    [XmlElement("activationAttribute", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public List<activationAttribute> activationAttribute { get; set; }

    [XmlElement("protectionKeyId", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public List<productKeyProtectionKeyId> protectionKeyId { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public object entitlementId { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public object customerId { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class keyProduct
  {
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class associatedAttribute
  {
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string attributeName { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string attributeValue { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public object readOnly { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [XmlType(AnonymousType = true)]
  public enum productKeyRegistrationRequired
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
  [XmlType(AnonymousType = true)]
  public class productKeyProductInfo : keyProduct
  {
    [XmlAttribute()]
    public string id { get; set; }

    [XmlAttribute()]
    public string productName { get; set; }

    [XmlAttribute()]
    public string type { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class productKeyAid
  {
    [XmlAttribute()]
    public string protectionKeyId { get; set; }

    [XmlText()]
    public string value { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class productKeyProtectionKeyId
  {
    private bool? _isDriverless;

    [XmlAttribute()]
    public string keyType { get; set; }

    [XmlAttribute()]
    public bool isDriverless
    {
      get
      {
        if (this._isDriverless.HasValue)
        {
          return this._isDriverless.Value;
        }
        else
        {
          return default(bool);
        }
      }
      set
      {
        this._isDriverless = value;
      }
    }

    [XmlIgnore()]
    public bool isDriverlessSpecified
    {
      get
      {
        return this._isDriverless.HasValue;
      }
      set
      {
        if (value == false)
        {
          this._isDriverless = null;
        }
      }
    }

    [XmlText()]
    public string value { get; set; }
  }
}
