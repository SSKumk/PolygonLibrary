using CGLibrary.Toolkit;
using NUnit.Framework;

namespace Tests.DoubleGeometry.Toolkit.Hashes;

[TestFixture]
public class HashesTests {

  [Test]
  public void GetMd5Hash_EmptyString_ReturnsKnownDigest() {
    Assert.That(CGLibrary.Toolkit.Hashes.GetMd5Hash(string.Empty), Is.EqualTo("d41d8cd98f00b204e9800998ecf8427e"));
  }

  [Test]
  public void GetMd5Hash_AsciiString_ReturnsKnownDigest() {
    Assert.That(CGLibrary.Toolkit.Hashes.GetMd5Hash("abc"), Is.EqualTo("900150983cd24fb0d6963f7d28e17f72"));
  }

  [Test]
  public void GetMd5Hash_UsesUtf8Encoding() {
    Assert.That(CGLibrary.Toolkit.Hashes.GetMd5Hash("привет"), Is.EqualTo("608333adc72f545078ede3aad71bfe74"));
  }

}
