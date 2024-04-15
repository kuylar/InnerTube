// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: search_response.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace InnerTube.Protobuf.Responses {

  /// <summary>Holder for reflection information generated from search_response.proto</summary>
  public static partial class SearchResponseReflection {

    #region Descriptor
    /// <summary>File descriptor for search_response.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SearchResponseReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChVzZWFyY2hfcmVzcG9uc2UucHJvdG8SHElubmVyVHViZS5Qcm90b2J1Zi5S",
            "ZXNwb25zZXMaDWdlbmVyYWwucHJvdG8i1QEKDlNlYXJjaFJlc3BvbnNlEhgK",
            "EGVzdGltYXRlZFJlc3VsdHMYAiABKAMSNQoIY29udGVudHMYBCABKAsyIy5J",
            "bm5lclR1YmUuUHJvdG9idWYuUmVuZGVyZXJXcmFwcGVyEhYKDnRyYWNraW5n",
            "UGFyYW1zGAkgASgMEjMKBmhlYWRlchgNIAEoCzIjLklubmVyVHViZS5Qcm90",
            "b2J1Zi5SZW5kZXJlcldyYXBwZXISEwoLcmVmaW5lbWVudHMYFiADKAkSEAoI",
            "dGFyZ2V0SWQYHiABKAliBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::InnerTube.Protobuf.GeneralReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::InnerTube.Protobuf.Responses.SearchResponse), global::InnerTube.Protobuf.Responses.SearchResponse.Parser, new[]{ "EstimatedResults", "Contents", "TrackingParams", "Header", "Refinements", "TargetId" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class SearchResponse : pb::IMessage<SearchResponse>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<SearchResponse> _parser = new pb::MessageParser<SearchResponse>(() => new SearchResponse());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<SearchResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::InnerTube.Protobuf.Responses.SearchResponseReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SearchResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SearchResponse(SearchResponse other) : this() {
      estimatedResults_ = other.estimatedResults_;
      contents_ = other.contents_ != null ? other.contents_.Clone() : null;
      trackingParams_ = other.trackingParams_;
      header_ = other.header_ != null ? other.header_.Clone() : null;
      refinements_ = other.refinements_.Clone();
      targetId_ = other.targetId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SearchResponse Clone() {
      return new SearchResponse(this);
    }

    /// <summary>Field number for the "estimatedResults" field.</summary>
    public const int EstimatedResultsFieldNumber = 2;
    private long estimatedResults_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public long EstimatedResults {
      get { return estimatedResults_; }
      set {
        estimatedResults_ = value;
      }
    }

    /// <summary>Field number for the "contents" field.</summary>
    public const int ContentsFieldNumber = 4;
    private global::InnerTube.Protobuf.RendererWrapper contents_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::InnerTube.Protobuf.RendererWrapper Contents {
      get { return contents_; }
      set {
        contents_ = value;
      }
    }

    /// <summary>Field number for the "trackingParams" field.</summary>
    public const int TrackingParamsFieldNumber = 9;
    private pb::ByteString trackingParams_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString TrackingParams {
      get { return trackingParams_; }
      set {
        trackingParams_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "header" field.</summary>
    public const int HeaderFieldNumber = 13;
    private global::InnerTube.Protobuf.RendererWrapper header_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::InnerTube.Protobuf.RendererWrapper Header {
      get { return header_; }
      set {
        header_ = value;
      }
    }

    /// <summary>Field number for the "refinements" field.</summary>
    public const int RefinementsFieldNumber = 22;
    private static readonly pb::FieldCodec<string> _repeated_refinements_codec
        = pb::FieldCodec.ForString(178);
    private readonly pbc::RepeatedField<string> refinements_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<string> Refinements {
      get { return refinements_; }
    }

    /// <summary>Field number for the "targetId" field.</summary>
    public const int TargetIdFieldNumber = 30;
    private string targetId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string TargetId {
      get { return targetId_; }
      set {
        targetId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as SearchResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(SearchResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (EstimatedResults != other.EstimatedResults) return false;
      if (!object.Equals(Contents, other.Contents)) return false;
      if (TrackingParams != other.TrackingParams) return false;
      if (!object.Equals(Header, other.Header)) return false;
      if(!refinements_.Equals(other.refinements_)) return false;
      if (TargetId != other.TargetId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (EstimatedResults != 0L) hash ^= EstimatedResults.GetHashCode();
      if (contents_ != null) hash ^= Contents.GetHashCode();
      if (TrackingParams.Length != 0) hash ^= TrackingParams.GetHashCode();
      if (header_ != null) hash ^= Header.GetHashCode();
      hash ^= refinements_.GetHashCode();
      if (TargetId.Length != 0) hash ^= TargetId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (EstimatedResults != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(EstimatedResults);
      }
      if (contents_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Contents);
      }
      if (TrackingParams.Length != 0) {
        output.WriteRawTag(74);
        output.WriteBytes(TrackingParams);
      }
      if (header_ != null) {
        output.WriteRawTag(106);
        output.WriteMessage(Header);
      }
      refinements_.WriteTo(output, _repeated_refinements_codec);
      if (TargetId.Length != 0) {
        output.WriteRawTag(242, 1);
        output.WriteString(TargetId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (EstimatedResults != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(EstimatedResults);
      }
      if (contents_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Contents);
      }
      if (TrackingParams.Length != 0) {
        output.WriteRawTag(74);
        output.WriteBytes(TrackingParams);
      }
      if (header_ != null) {
        output.WriteRawTag(106);
        output.WriteMessage(Header);
      }
      refinements_.WriteTo(ref output, _repeated_refinements_codec);
      if (TargetId.Length != 0) {
        output.WriteRawTag(242, 1);
        output.WriteString(TargetId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (EstimatedResults != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(EstimatedResults);
      }
      if (contents_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Contents);
      }
      if (TrackingParams.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(TrackingParams);
      }
      if (header_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Header);
      }
      size += refinements_.CalculateSize(_repeated_refinements_codec);
      if (TargetId.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(TargetId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(SearchResponse other) {
      if (other == null) {
        return;
      }
      if (other.EstimatedResults != 0L) {
        EstimatedResults = other.EstimatedResults;
      }
      if (other.contents_ != null) {
        if (contents_ == null) {
          Contents = new global::InnerTube.Protobuf.RendererWrapper();
        }
        Contents.MergeFrom(other.Contents);
      }
      if (other.TrackingParams.Length != 0) {
        TrackingParams = other.TrackingParams;
      }
      if (other.header_ != null) {
        if (header_ == null) {
          Header = new global::InnerTube.Protobuf.RendererWrapper();
        }
        Header.MergeFrom(other.Header);
      }
      refinements_.Add(other.refinements_);
      if (other.TargetId.Length != 0) {
        TargetId = other.TargetId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 16: {
            EstimatedResults = input.ReadInt64();
            break;
          }
          case 34: {
            if (contents_ == null) {
              Contents = new global::InnerTube.Protobuf.RendererWrapper();
            }
            input.ReadMessage(Contents);
            break;
          }
          case 74: {
            TrackingParams = input.ReadBytes();
            break;
          }
          case 106: {
            if (header_ == null) {
              Header = new global::InnerTube.Protobuf.RendererWrapper();
            }
            input.ReadMessage(Header);
            break;
          }
          case 178: {
            refinements_.AddEntriesFrom(input, _repeated_refinements_codec);
            break;
          }
          case 242: {
            TargetId = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 16: {
            EstimatedResults = input.ReadInt64();
            break;
          }
          case 34: {
            if (contents_ == null) {
              Contents = new global::InnerTube.Protobuf.RendererWrapper();
            }
            input.ReadMessage(Contents);
            break;
          }
          case 74: {
            TrackingParams = input.ReadBytes();
            break;
          }
          case 106: {
            if (header_ == null) {
              Header = new global::InnerTube.Protobuf.RendererWrapper();
            }
            input.ReadMessage(Header);
            break;
          }
          case 178: {
            refinements_.AddEntriesFrom(ref input, _repeated_refinements_codec);
            break;
          }
          case 242: {
            TargetId = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
