# C# / .NET NuGet 패키지 관리 가이드

## 1. 개요

NuGet은 .NET 생태계에서 사용하는 대표적인 패키지 관리자이다.

NuGet은 특정한 공식 약자라기보다는 이름에 가깝지만, 의미상 다음처럼 이해할 수 있다.

```text
Nu
  - New 또는 NuGet 고유 이름의 일부로 해석할 수 있다.

Get
  - 패키지를 가져온다(get)는 의미로 이해할 수 있다.
```

즉, NuGet은 다음 역할을 수행한다.

```text
.NET 프로젝트에서 필요한 외부 라이브러리 패키지를
검색, 설치, 복원, 업데이트, 삭제, 게시할 수 있게 해주는 패키지 관리 시스템
```

다른 언어 생태계와 비교하면 다음과 비슷한 역할이다.

| 생태계 | 패키지 관리자 |
|---|---|
| .NET / C# | NuGet |
| Node.js | npm |
| Python | pip |
| Java | Maven / Gradle |
| Rust | Cargo |

NuGet 패키지 관리는 프로젝트 스타일과 설정 방식에 따라 크게 다음 방식으로 나뉜다.

```text
1. packages.config 방식
2. PackageReference 방식
3. CPM(Central Package Management) 방식
```

---

## 2. NuGet 패키지 관리 방식

### 2.1. NuGet 패키지 관리

NuGet 패키지 관리는 프로젝트에서 외부 패키지를 참조하고, 복원하고, 업데이트하는 기본 기능을 의미한다.

대표적인 방식은 다음과 같다.

```text
1. packages.config + Reference + HintPath 방식
2. PackageReference 방식
3. NuGet 패키지 관리 메뉴 사용
```

---

#### 2.1.1. Package.Config 파일 & Reference + HintPath 속성 설정 활용

`packages.config` 방식은 주로 구형 `.NET Framework Style` 프로젝트에서 사용한다.

이 방식에서는 패키지 정보와 실제 DLL 참조가 분리된다.

```text
packages.config
  - 설치된 NuGet 패키지 ID와 Version 기록

.csproj
  - 실제 컴파일에 필요한 DLL을 Reference + HintPath로 참조
```

예를 들어 `packages.config`에는 다음처럼 기록된다.

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="MSTest.TestFramework" version="4.2.2" targetFramework="net481" />
</packages>
```

그리고 `.csproj`에는 실제 DLL 참조가 들어간다.

```xml
<ItemGroup>
  <Reference Include="MSTest.TestFramework, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL">
    <HintPath>..\packages\MSTest.TestFramework.4.2.2\lib\net462\MSTest.TestFramework.dll</HintPath>
  </Reference>

  <Reference Include="MSTest.TestFramework.Extensions, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL">
    <HintPath>..\packages\MSTest.TestFramework.4.2.2\lib\net462\MSTest.TestFramework.Extensions.dll</HintPath>
  </Reference>
</ItemGroup>
```

이 방식에서 중요한 점은 다음이다.

```text
packages.config
  → 어떤 패키지와 버전을 사용할지 기록

Reference + HintPath
  → 실제 컴파일할 DLL 위치 기록

Solution/packages
  → 실제 패키지 파일 저장 위치
```

따라서 `packages.config`의 버전만 수동으로 바꾸면 안된다.

예를 들어 `packages.config`만 `3.6.4`로 바꾸고,
`.csproj`의 `HintPath`는 여전히 `4.2.2`를 가리키면 패키지 삭제/복원/빌드가 꼬일 수 있다.

버전 변경은 NuGet 패키지 관리 UI 또는 `Update-Package` 명령을 사용해야 한다.

```powershell
Update-Package MSTest.TestFramework -Version 3.6.4 -ProjectName CSharp
```

---

#### 2.1.2. PackageReference 속성 설정 활용

`PackageReference` 방식은 현재 .NET 프로젝트에서 일반적으로 사용하는 방식이다.

패키지 참조가 `.csproj`에 직접 들어간다.

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
  <PackageReference Include="NLog" Version="6.1.2" />
</ItemGroup>
```

이 방식에서는 직접 `Reference + HintPath`를 관리하지 않는다.

NuGet Restore가 다음을 자동으로 처리한다.

```text
1. 패키지 다운로드
2. 대상 TargetFramework에 맞는 DLL 선택
3. 전이 종속성 계산
4. obj/project.assets.json 생성
5. 빌드 시 필요한 참조 구성
```

패키지는 기본적으로 전역 NuGet 캐시에 복원된다.

```text
C:\Users\<UserName>\.nuget\packages
```

`PackageReference` 방식은 SDK-style 프로젝트의 기본 방식이다.

---

#### 2.1.3. NuGet 패키지 관리 메뉴 사용

Visual Studio에서는 다음 메뉴를 통해 NuGet 패키지를 관리할 수 있다.

```text
프로젝트 우클릭
→ NuGet 패키지 관리
```

또는:

```text
솔루션 우클릭
→ 솔루션용 NuGet 패키지 관리
```

프로젝트 방식에 따라 NNuGet 패키지 관리 UI의 동작이 달라진다.

| 프로젝트 방식 | NuGet 패키지 관리 UI 설치 결과 |
|---|---|
| packages.config 방식 | `packages.config` 수정 + `.csproj`에 `Reference + HintPath` 추가 |
| PackageReference 방식 | `.csproj`에 `PackageReference` 추가 |
| CPM 방식 | `.csproj`에 `PackageReference` 추가, 버전은 `Directory.Packages.props` 수정 필요 |

주의할 점은 CPM 사용 시 NuGet 패키지 관리 UI가 `.csproj`에 `Version`을 넣을 수 있다는 점이다.

```xml
<PackageReference Include="MSTest.TestFramework">
  <Version>4.2.2</Version>
</PackageReference>
```

CPM에서는 이 형태가 잘못된 설정이다.

CPM 사용 시에는 `.csproj`에 버전을 쓰면 안된다.
따라서 아래의 내용처럼 Version을 제거해야 한다.

```xml
<PackageReference Include="MSTest.TestFramework" />
```

버전은 `Directory.Packages.props`에 작성한다.

```xml
<PackageVersion Include="MSTest.TestFramework" Version="4.2.2" />
```

---

### 2.2. 중앙 패키지 관리 (CPM) + NuGet 패키지 관리

CPM은 `PackageReference` 기반 프로젝트에서 패키지 버전을 중앙에서 관리하는 방식이다.

즉, CPM은 `PackageReference`를 대체하는 방식이 아니다.

```text
PackageReference
  → 프로젝트가 어떤 패키지를 사용할지 선언하는 방식

CPM
  → PackageReference의 버전을 중앙에서 관리하는 방식
```

따라서 모든 CPM 프로젝트는 `PackageReference`를 사용하지만, 모든 `PackageReference` 프로젝트가 CPM인 것은 아니다.

---

#### 2.2.1. 중앙 패키지 관리 기능 CPM (Central Package Management)

CPM을 사용하면 각 프로젝트에 패키지 버전을 직접 쓰지 않는다.

각 프로젝트는 패키지 사용 여부만 선언한다.

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" />
  <PackageReference Include="NLog" />
</ItemGroup>
```

버전은 `Directory.Packages.props`에서 중앙 관리한다.

```xml
<PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
<PackageVersion Include="NLog" Version="6.1.2" />
```

이 구조의 장점은 다음이다.

```text
1. 여러 프로젝트의 패키지 버전을 한 곳에서 통일할 수 있다.
2. 버전 충돌을 줄일 수 있다.
3. 솔루션 전체 패키지 업그레이드를 관리하기 쉽다.
4. 각 프로젝트 파일이 간결해진다.
```

---

##### 2.2.1.1. Directory.Packages.props 파일 설정

`Directory.Packages.props`는 CPM에서 사용하는 핵심 파일이다.

예:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageVersionOverrideEnabled>true</CentralPackageVersionOverrideEnabled>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
    <PackageVersion Include="NLog" Version="6.1.2" />
    <PackageVersion Include="MSTest.TestFramework" Version="4.2.2" />
    <PackageVersion Include="MSTest.TestAdapter" Version="4.2.2" />
  </ItemGroup>
</Project>
```

주요 역할은 다음이다.

```text
ManagePackageVersionsCentrally
  → 중앙 패키지 버전 관리 활성화

PackageVersion
  → 중앙에서 관리할 패키지 버전 정의

CentralPackageVersionOverrideEnabled
  → 개별 프로젝트에서 VersionOverride 허용 여부 설정
```

`Directory.Packages.props`는 `.sln` 기준이 아니라 각 `.csproj` 기준으로 상위 경로에서 탐색된다.

---

##### 2.2.1.2. 프로젝트별 버전 재정의 설정

CPM을 사용하는 상태에서 특정 프로젝트만 다른 버전을 쓰고 싶으면 `VersionOverride`를 사용할 수 있다.

상위 `Directory.Packages.props`:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageVersionOverrideEnabled>true</CentralPackageVersionOverrideEnabled>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
  </ItemGroup>
</Project>
```

특정 프로젝트 `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" VersionOverride="13.0.3" />
</ItemGroup>
```

이 경우 해당 프로젝트만 `Newtonsoft.Json 13.0.3`을 사용한다.

주의할 점은 `VersionOverride`와 `Version`은 다르다는 것이다.

CPM에서 아래는 잘못된 방식이다.

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

CPM에서 특정 프로젝트 전체를 중앙 패키지 관리 기능(CPM)에서 제외시키고 싶으면 다음 설정을 사용한다.

```xml
<PropertyGroup>
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>
```

이 경우 해당 프로젝트는 일반 `PackageReference` 방식으로 동작한다.

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

즉:

```text
VersionOverride
  → CPM은 유지하면서 특정 패키지만 예외 버전 사용

ManagePackageVersionsCentrally=false
  → 해당 프로젝트 전체를 CPM에서 제외
  → 일반 PackageReference 방식으로 동작
```

---

## 3. 주요 속성

### `Reference`

DLL 또는 어셈블리를 직접 참조하는 MSBuild 항목이다.

```xml
<Reference Include="MyLibrary">
  <HintPath>..\libs\MyLibrary.dll</HintPath>
</Reference>
```

`packages.config` 방식에서는 NuGet 패키지 DLL도 `Reference + HintPath`로 들어간다.

### `HintPath`

`Reference`가 참조할 실제 DLL 경로이다.

```xml
<HintPath>..\packages\Newtonsoft.Json.13.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
```

`HintPath`가 실제 파일 위치와 다르면 컴파일 참조가 깨질 수 있다.

### `PackageReference`

NuGet 패키지를 프로젝트에서 사용하겠다고 선언하는 항목이다.

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
```

CPM 사용 시에는 버전을 쓰지 않는다.

```xml
<PackageReference Include="Newtonsoft.Json" />
```

### `PackageVersion`

CPM에서 패키지 버전을 중앙 관리하기 위한 항목이다.

```xml
<PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
```

`Directory.Packages.props`에 작성한다.

### `ManagePackageVersionsCentrally`

CPM을 활성화하거나 비활성화하는 속성이다.

```xml
<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
```

특정 프로젝트만 CPM에서 제외하려면 `.csproj`에 다음을 둔다.

```xml
<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
```

이 경우 NuGet 기능이 꺼지는 것이 아니라, 중앙 버전 관리만 꺼진다.

### `CentralPackageVersionOverrideEnabled`

`VersionOverride` 사용 가능 여부를 설정한다.

```xml
<CentralPackageVersionOverrideEnabled>true</CentralPackageVersionOverrideEnabled>
```

이 설정이 `false`이면 다음 설정을 사용할 수 없다.

```xml
<PackageReference Include="Newtonsoft.Json" VersionOverride="13.0.3" />
```

### `VersionOverride`

CPM 사용 중 특정 프로젝트만 특정 패키지 버전을 재정의할 때 사용한다.

```xml
<PackageReference Include="Newtonsoft.Json" VersionOverride="13.0.3" />
```

### `RestoreProjectStyle`

구형 `.NET Framework Style` 프로젝트에서 `PackageReference` 방식으로 복원하게 강제하는 속성이다.

```xml
<RestoreProjectStyle>PackageReference</RestoreProjectStyle>
```

이 설정이 있으면 NuGet은 해당 프로젝트를 `packages.config` 방식이 아니라 `PackageReference` 방식으로 본다.

### `NuGetPackageImportStamp`

`packages.config` 방식에서 NuGet이 `.props`, `.targets` import 처리를 하면서 추가할 수 있는 내부 관리용 표식이다.

```xml
<NuGetPackageImportStamp>
</NuGetPackageImportStamp>
```

비어 있어도 정상이다.

---

## 4. 프로젝트 스타일별 패키지 관리 설정

### 4.2. .NET Framework Style

#### 4.2.1. 개요

`.NET Framework Style` 프로젝트는 구형 `.csproj` 형식이다.

예:

```xml
<Project ToolsVersion="14.0"
         DefaultTargets="Build"
         xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
```

이 프로젝트 스타일에서는 다음 방식이 가능하다.

```text
1. packages.config + Reference + HintPath 방식
2. PackageReference 방식
3. CPM 방식
```

다만 기본적으로는 `packages.config` 파일 활용 방식과 많이 함께 사용된다.

---

#### 4.2.2. Package.Config + Reference + HintPath 적용

이 방식에서는 NuGet 패키지 관리 UI로 패키지를 설치하면 다음이 자동으로 추가된다.

```text
packages.config
.csproj Reference + HintPath
필요한 Import / Target / Analyzer
```

예:

```xml
<!-- packages.config -->
<package id="MSTest.TestFramework" version="4.2.2" targetFramework="net481" />
```

```xml
<!-- .csproj -->
<Reference Include="MSTest.TestFramework, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL">
  <HintPath>..\packages\MSTest.TestFramework.4.2.2\lib\net462\MSTest.TestFramework.dll</HintPath>
</Reference>
```

이 방식은 `Directory.Packages.props`와 연동되지 않는다.

패키지 버전 기준은 `packages.config`의 `version`이다.

---

#### 4.2.3. PackageReference 적용

구형 `.NET Framework Style` 프로젝트에서도 `PackageReference`를 사용할 수 있다.

```xml
<PropertyGroup>
  <RestoreProjectStyle>PackageReference</RestoreProjectStyle>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

이 경우 `packages.config`는 사용되지 않는다.

주의:

```text
RestoreProjectStyle=PackageReference만 추가하고
기존 packages.config 방식 참조를 남겨두면
NuGet 패키지 관리 UI의 설치됨 목록과 packages.config 내의 정보가 맞지 않는다.
```

따라서 하나의 프로젝트에서는 `packages.config` 방식과 `PackageReference` 방식을 혼용하지 말자 !!!

---

#### 4.2.4. 중앙 패키지 관리 기능 CPM (Central Package Management) 적용

`.NET Framework Style` 프로젝트에서도 `PackageReference` 기반이라면 CPM을 사용할 수 있다.

`Directory.Packages.props`:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
  </ItemGroup>
</Project>
```

`.csproj`:

```xml
<PropertyGroup>
  <RestoreProjectStyle>PackageReference</RestoreProjectStyle>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" />
</ItemGroup>
```

이 경우 `.csproj`에는 `Version`을 쓰지 않는다.

---

### 4.3. .NET SDK Framework Style

> 보통은 `.NET SDK-Style` 또는 `SDK-Style project`라고 부른다.

#### 4.3.1. 개요

SDK-Style 프로젝트는 다음처럼 시작한다.

```xml
<Project Sdk="Microsoft.NET.Sdk">
```

SDK-Style 프로젝트는 기본적으로 `PackageReference` 방식을 사용한다.

따라서 `packages.config` 방식은 제공하지 않는다.

---

#### 4.3.1. PackageReference 적용

중앙 패키지 관리를 사용하지 않는 경우 `.csproj`에 버전을 직접 작성한다.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
    <PackageReference Include="NLog" Version="6.1.2" />
  </ItemGroup>

</Project>
```

이 상태에서는 NuGet 패키지 관리 UI, `dotnet add package`, `dotnet remove package`, `dotnet restore`를 모두 사용할 수 있다.

---

#### 4.3.2. 중앙 패키지 관리 기능 CPM (Central Package Management) 적용

SDK-Style 프로젝트는 CPM과 자연스럽게 연동된다.

`Directory.Packages.props`:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
    <PackageVersion Include="NLog" Version="6.1.2" />
  </ItemGroup>
</Project>
```

`.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" />
    <PackageReference Include="NLog" />
  </ItemGroup>

</Project>
```

특정 SDK-Style 프로젝트만 CPM에서 제외하려면:

```xml
<PropertyGroup>
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>
```

그리고 패키지 버전은 `.csproj`에 직접 쓴다.

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

이 경우에도 NuGet 패키지 관리 UI, `dotnet add package`, `dotnet remove package`, `dotnet restore`는 정상적으로 사용할 수 있다.

---

## 5. NuGet 패키지 관리 설정

### 5.1. 개요

NuGet 설정은 주로 `nuget.config` 파일에서 관리한다.

`nuget.config`는 솔루션 폴더뿐 아니라 그보다 상위 폴더에도 둘 수 있다.

NuGet은 현재 작업 디렉터리와 상위 디렉터리의 `nuget.config`를 탐색하여 설정을 적용한다.

주요 설정은 다음과 같다.

```xml
<configuration>
  <config>
    <add key="globalPackagesFolder" value=".nuget\packages" />
    <add key="repositoryPath" value=".nuget\packages.config" />
  </config>
</configuration>
```

| 설정 | 적용 대상 | 의미 |
|---|---|---|
| `globalPackagesFolder` | PackageReference / SDK-Style / CPM | 전역 패키지 폴더 위치 |
| `repositoryPath` | packages.config | packages.config 방식 패키지 저장 위치 |
| `NUGET_PACKAGES` | PackageReference 계열 | `globalPackagesFolder`보다 우선될 수 있는 환경 변수 |

---

### 5.2. 개발 그룹 수준 NuGet 패키지 관리 설정 모델

솔루션보다 상위의 개발 그룹 루트에 `nuget.config`를 두고 관리할 수 있다.

예:

```text
MyDevRoot/
  ProjectGroup.A/
    nuget.config
    Directory.Packages.props
    Server/
      Server.sln
      GameServer/
        GameServer.csproj

  ProjectGroup.B/
    nuget.config
    Directory.Packages.props
    Server/
      Server.sln
      GameServer/
        GameServer.csproj
```

`ProjectGroup.A/nuget.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="globalPackagesFolder" value=".nuget\packages" />
    <add key="repositoryPath" value=".nuget\packages.config" />
  </config>
</configuration>
```

이 경우:

```text
MyDevRoot/ProjectGroup.A/Server/Server.sln
  → MyDevRoot/ProjectGroup.A/nuget.config 적용

MyDevRoot/ProjectGroup.B/Server/Server.sln
  → MyDevRoot/ProjectGroup.B/nuget.config 적용
```

결과 구조:

```text
ProjectGroup.A/
  .nuget/
    packages/          ← PackageReference / SDK-Style / CPM
    packages.config/   ← packages.config 방식
```

주의:

```text
NUGET_PACKAGES 환경 변수가 설정되어 있으면
nuget.config의 globalPackagesFolder보다 우선될 수 있다.
```

확인:

```powershell
echo $env:NUGET_PACKAGES
```

적용 경로 확인:

```powershell
dotnet nuget locals global-packages --list
```

### 5.3. nuget.config 설정 적용 하기

`nuget.config`를 작성했다고 기존 패키지 경로가 자동으로 모두 반영(Migration) 되는 것은 아니다.

`nuget.config`는 **이후 NuGet Restore / Install / Update 시 사용할 패키지 저장 경로 정책**을 정의한다.

따라서 설정 적용 과정은 다음 순서로 진행하는 것이 안전하다.

```text
1. 개발 그룹 루트에 nuget.config 배치
2. globalPackagesFolder / repositoryPath 설정
3. 기존 bin / obj 정리
4. 기존 패키지 캐시 또는 packages 폴더 정리 여부 결정
5. Restore 실행
6. 실제 적용 경로 확인
7. 빌드 확인
```

#### 5.3.1. 개발 그룹 루트에 nuget.config 배치

ProjectGroup.A 단위로 NuGet 패키지 경로를 분리하려면 다음 위치에 nuget.config를 둔다.

```text
MyDevRoot/
  ProjectGroup.A/
    nuget.config
    Directory.Packages.props
    Server/
      Server.sln
      LoginAuthServer/
        LoginAuth.csproj
```
ProjectGroup.A/nuget.config:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <!-- PackageReference / SDK-Style / CPM 방식 -->
    <add key="globalPackagesFolder" value=".nuget\packages" />

    <!-- packages.config 방식 -->
    <add key="repositoryPath" value=".nuget\packages.config" />
  </config>
</configuration>
```
이 설정의 의미는 다음과 같다.

```text
globalPackagesFolder
  → PackageReference / SDK-Style / CPM 프로젝트의 패키지 복원 경로

repositoryPath
  → packages.config 방식 프로젝트의 패키지 복원 경로
```

이 설정을 적용하면 패키지 저장 경로는 다음처럼 분리된다.

```text
ProjectGroup.A/
  .nuget/
    packages/          ← PackageReference / SDK-Style / CPM 방식
    packages.config/   ← packages.config 방식
```

즉, 같은 `ProjectGroup.A` 하위에 있더라도 NuGet 관리 방식에 따라 서로 다른 저장 경로를 사용한다.

```text
PackageReference / SDK-Style / CPM
  → globalPackagesFolder 설정 사용
  → ProjectGroup.A/.nuget/packages 사용

packages.config
  → repositoryPath 설정 사용
  → ProjectGroup.A/.nuget/packages.config 사용
```

이렇게 분리하는 이유는 두 방식의 패키지 폴더 구조가 다르기 때문이다.

`PackageReference / SDK-Style / CPM` 방식은 보통 다음과 같은 구조를 사용한다.

```text
.nuget/
  packages/
    newtonsoft.json/
      13.0.4/
        lib/
        newtonsoft.json.nuspec
```

반면 `packages.config` 방식은 보통 다음과 같은 구조를 사용한다.

```text
.nuget/
  packages.config/
    Newtonsoft.Json.13.0.4/
      lib/
      Newtonsoft.Json.nuspec
```

따라서 두 방식을 같은 폴더에 섞기보다는 다음처럼 분리하는 것이 관리하기 쉽다.

```xml
<add key="globalPackagesFolder" value=".nuget\packages" />
<add key="repositoryPath" value=".nuget\packages.config" />
```

---

#### 5.3.2. PackageReference / SDK-Style / CPM 프로젝트에 적용하기

`PackageReference`, `SDK-Style`, `CPM` 방식 프로젝트는 `globalPackagesFolder` 설정의 영향을 받는다.

```xml
<add key="globalPackagesFolder" value=".nuget\packages" />
```

이 설정이 있으면 패키지는 다음 위치로 복원된다.

```text
ProjectGroup.A/
  .nuget/
    packages/
      newtonsoft.json/
        13.0.4/
```

이 방식에서는 `.csproj`에 직접 DLL 경로가 들어가지 않는다.

일반 `PackageReference` 프로젝트는 다음처럼 유지된다.

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

CPM 프로젝트는 다음처럼 유지된다.

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" />
</ItemGroup>
```

그리고 버전은 `Directory.Packages.props`에 둔다.

```xml
<ItemGroup>
  <PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

즉, `globalPackagesFolder`를 변경해도 일반적으로 다음 파일들은 수정하지 않는다.

```text
수정하지 않음:
  - .csproj의 PackageReference
  - Directory.Packages.props의 PackageVersion
  - Reference + HintPath 없음
```

적용하려면 restore를 다시 실행한다.

```powershell
cd MyDevRoot\ProjectGroup.A
dotnet restore Server\Server.sln
```

또는 솔루션 폴더에서 실행한다.

```powershell
cd MyDevRoot\ProjectGroup.A\Server
dotnet restore Server.sln
```

적용된 global packages 경로를 확인한다.

```powershell
dotnet nuget locals global-packages --list
```

예상 결과:

```text
global-packages: MyDevRoot\ProjectGroup.A\.nuget\packages
```

---

#### 5.3.3. packages.config 프로젝트에 적용하기

`packages.config` 방식 프로젝트는 `repositoryPath` 설정의 영향을 받는다.

```xml
<add key="repositoryPath" value=".nuget\packages.config" />
```

이 설정이 있으면 packages.config 방식 패키지는 다음 위치로 복원된다.

```text
ProjectGroup.A/
  .nuget/
    packages.config/
      MSTest.TestFramework.4.2.2/
```

하지만 `packages.config` 방식은 주의가 필요하다.

이 방식에서는 `.csproj`의 `Reference + HintPath`가 실제 DLL 경로를 직접 가리킨다.

예를 들어 기존 `.csproj`가 다음처럼 되어 있었다면:

```xml
<Reference Include="MSTest.TestFramework, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL">
  <HintPath>..\packages\MSTest.TestFramework.4.2.2\lib\net462\MSTest.TestFramework.dll</HintPath>
</Reference>
```

패키지 복원 경로를 `.nuget\packages.config`로 바꾸면 `HintPath`도 실제 위치와 맞아야 한다.

예를 들어 프로젝트 위치가 다음과 같다면:

```text
ProjectGroup.A/
  .nuget/
    packages.config/
      MSTest.TestFramework.4.2.2/
  Server/
    LoginAuthServer/
      LoginAuth.csproj
      packages.config
```

`LoginAuth.csproj` 기준으로 패키지 경로는 다음처럼 될 수 있다.

```xml
<Reference Include="MSTest.TestFramework, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL">
  <HintPath>..\..\.nuget\packages.config\MSTest.TestFramework.4.2.2\lib\net462\MSTest.TestFramework.dll</HintPath>
</Reference>
```

즉, packages.config 방식에서 경로 변경 시 확인할 항목은 다음이다.

| 확인 대상 | 설명 |
|---|---|
| `nuget.config`의 `repositoryPath` | packages.config 방식 패키지 복원 위치 |
| `.csproj`의 `Reference HintPath` | 실제 DLL 참조 경로 |
| `.csproj`의 `Import Project` | 패키지 `.props`, `.targets` 경로 |
| `.csproj`의 `Analyzer Include` | Analyzer 패키지 경로 |
| `EnsureNuGetPackageBuildImports` | build 파일 존재 여부 검사 경로 |
| `packages.config` | 패키지 ID / Version 목록 |

`packages.config` 자체는 보통 수정하지 않는다.

`packages.config`는 패키지 ID와 버전을 기록하는 파일이고, 실제 DLL 참조 경로는 `.csproj`의 `HintPath`가 결정한다.

---

#### 5.3.4. 기존 경로에서 새 경로로 이전하는 방법

##### 5.3.4.1. PackageReference / SDK-Style / CPM 방식

이 방식은 비교적 단순하다.

```text
1. ProjectGroup.A/nuget.config 작성
2. globalPackagesFolder 설정
3. bin / obj 삭제 권장
4. dotnet restore 실행
5. dotnet nuget locals global-packages --list 확인
6. 빌드
```

예:

```powershell
cd MyDevRoot\ProjectGroup.A

Remove-Item -Recurse -Force .\Server\LoginAuthServer\bin -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force .\Server\LoginAuthServer\obj -ErrorAction SilentlyContinue

dotnet restore .\Server\Server.sln
dotnet build .\Server\Server.sln
```

##### 5.3.4.2. packages.config 방식

이 방식은 `.csproj`의 경로 참조가 있기 때문에 더 조심해야 한다.

권장 절차는 다음이다.

```text
1. ProjectGroup.A/nuget.config 작성
2. repositoryPath 설정
3. NuGet 패키지 관리 UI 또는 Package Manager Console로 패키지 재설치
4. .csproj의 HintPath / Import / Analyzer 경로 확인
5. 빌드
```

Package Manager Console에서 재설치:

```powershell
Update-Package -Reinstall -ProjectName LoginAuth
```

또는 패키지를 제거 후 다시 설치한다.

```powershell
Uninstall-Package MSTest.TestFramework -ProjectName LoginAuth
Install-Package MSTest.TestFramework -Version 4.2.2 -ProjectName LoginAuth
```

이 과정을 거치면 NuGet이 새 `repositoryPath` 기준으로 `.csproj`의 `HintPath`를 다시 작성할 수 있다.

단, 프로젝트 상태나 패키지 종류에 따라 일부 `Import`, `Analyzer`, `EnsureNuGetPackageBuildImports` 경로는 직접 확인하는 것이 안전하다.

---

#### 5.3.5. 환경 변수 확인

`NUGET_PACKAGES` 환경 변수가 설정되어 있으면 `globalPackagesFolder`보다 우선될 수 있다.

따라서 `ProjectGroup.A/.nuget/packages`로 복원되기를 기대했는데 다른 위치로 복원된다면 먼저 환경 변수를 확인한다.

```powershell
echo $env:NUGET_PACKAGES
```

값이 있다면 현재 PowerShell 세션에서 제거할 수 있다.

```powershell
Remove-Item Env:NUGET_PACKAGES
```

Windows 사용자 또는 시스템 환경 변수에 등록되어 있다면 환경 변수 설정에서 제거한 뒤 Visual Studio와 터미널을 다시 시작해야 한다.

---

#### 5.3.6. 적용 확인 명령어

PackageReference / SDK-Style / CPM의 패키지 경로 확인:

```powershell
dotnet nuget locals global-packages --list
```

전체 NuGet 캐시 경로 확인:

```powershell
dotnet nuget locals all --list
```

복원 실행:

```powershell
dotnet restore
```

솔루션 단위 복원:

```powershell
dotnet restore Server.sln
```

packages.config 방식 복원:

```powershell
nuget restore Server.sln
```

packages.config 방식에서 패키지 폴더 지정 복원:

```powershell
nuget restore Server.sln -PackagesDirectory .nuget\packages.config
```

---

#### 5.3.7. Git 관리

패키지 캐시 폴더는 일반적으로 Git에 포함하지 않는다.

`.gitignore` 예:

```gitignore
.nuget/packages/
.nuget/packages.config/
```

반대로 `nuget.config`는 팀원들이 같은 패키지 경로 정책을 사용해야 하므로 Git에 포함하는 것이 좋다.

```text
Git 포함:
  nuget.config
  Directory.Packages.props

Git 제외:
  .nuget/packages/
  .nuget/packages.config/
```

---

## 6. NuGet 패키지 관리 주요 기능

NuGet 패키지 관리의 주요 기능은 다음과 같다.

```text
1. 패키지 설치
2. 패키지 삭제
3. 패키지 업데이트 / 다운그레이드
4. 패키지 복원
5. 설치된 패키지 목록 확인
6. 오래된 패키지 확인
7. 취약점 패키지 확인
8. 패키지 캐시 확인 / 삭제
9. 패키지 소스 관리
10. 패키지 생성 / 게시
```

방식별로 사용하는 도구가 다르다.

| 방식 | 권장 도구 |
|---|---|
| SDK-Style / PackageReference | `dotnet` CLI, NuGet 패키지 관리 UI |
| CPM | `dotnet` CLI, NuGet 패키지 관리 UI, `Directory.Packages.props` 수동 관리 |
| packages.config | NuGet 패키지 관리 UI, Package Manager Console, `nuget.exe restore` |

---

## 7. NuGet 명령어

### 7.1. .NET CLI 주요 명령어

패키지 설치:

```powershell
dotnet add package Newtonsoft.Json
```

특정 프로젝트에 설치:

```powershell
dotnet add CSharp.csproj package Newtonsoft.Json
```

버전 지정:

```powershell
dotnet add package Newtonsoft.Json --version 13.0.4
```

패키지 삭제:

```powershell
dotnet remove package Newtonsoft.Json
```

복원:

```powershell
dotnet restore
```

패키지 목록:

```powershell
dotnet list package
```

오래된 패키지 확인:

```powershell
dotnet list package --outdated
```

전이 종속성 포함:

```powershell
dotnet list package --include-transitive
```

취약점 확인:

```powershell
dotnet list package --vulnerable
```

NuGet 캐시 위치 확인:

```powershell
dotnet nuget locals all --list
```

NuGet 캐시 삭제:

```powershell
dotnet nuget locals all --clear
```

NuGet source 추가:

```powershell
dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
```

NuGet source 목록:

```powershell
dotnet nuget list source
```

패키지 생성:

```powershell
dotnet pack -c Release
```

패키지 게시:

```powershell
dotnet nuget push .\nupkgs\MyLibrary.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

---

### 7.2. NuGet 주요 명령어

`nuget.exe`는 구형 `.NET Framework`, `packages.config`, 패키지 제작/복원에서 많이 사용한다.

솔루션 복원:

```powershell
nuget restore YourSolution.sln
```

패키지 저장 폴더 지정:

```powershell
nuget restore YourSolution.sln -PackagesDirectory packages
```

`packages.config` 직접 복원:

```powershell
nuget restore CSharp\packages.config -PackagesDirectory packages
```

패키지 다운로드:

```powershell
nuget install Newtonsoft.Json -Version 13.0.4 -OutputDirectory packages
```

주의:

```text
nuget install은 보통 패키지를 폴더에 내려받는 명령이다.
.csproj에 Reference를 자동 추가하는 프로젝트 설치 명령으로 이해하면 안 된다.
```

패키지 소스 목록:

```powershell
nuget sources List
```

패키지 소스 추가:

```powershell
nuget sources Add -Name CompanyNuGet -Source https://nuget.mycompany.com/v3/index.json
```

NuGet 캐시 확인:

```powershell
nuget locals all -list
```

NuGet 캐시 삭제:

```powershell
nuget locals all -clear
```

패키지 생성:

```powershell
nuget pack MyLibrary.csproj -OutputDirectory .\nupkgs
```

패키지 게시:

```powershell
nuget push .\nupkgs\MyLibrary.1.0.0.nupkg -ApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
```

---

### 7.3. Visual Studio Package Manager Console 주요 명령어

패키지 설치:

```powershell
Install-Package Newtonsoft.Json
```

특정 프로젝트에 설치:

```powershell
Install-Package Newtonsoft.Json -ProjectName CSharp
```

버전 지정:

```powershell
Install-Package Newtonsoft.Json -Version 13.0.4 -ProjectName CSharp
```

패키지 제거:

```powershell
Uninstall-Package Newtonsoft.Json -ProjectName CSharp
```

패키지 업데이트:

```powershell
Update-Package Newtonsoft.Json -ProjectName CSharp
```

특정 버전으로 업데이트 / 다운그레이드:

```powershell
Update-Package Newtonsoft.Json -Version 13.0.4 -ProjectName CSharp
```

packages.config 방식 재설치:

```powershell
Update-Package -Reinstall -ProjectName CSharp
```

설치된 패키지 확인:

```powershell
Get-Package -ProjectName CSharp
```

업데이트 가능한 패키지 확인:

```powershell
Get-Package -Updates
```

---

## 8. NuGet 명령어 사용하기

---

### 8.1. .NET SDK-Style + PackageReference Only

중앙 패키지 관리를 사용하지 않는 SDK-Style 프로젝트 예:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
  </ItemGroup>

</Project>
```

패키지 설치:

```powershell
dotnet add package Newtonsoft.Json --version 13.0.4
```

패키지 삭제:

```powershell
dotnet remove package Newtonsoft.Json
```

복원:

```powershell
dotnet restore
```

목록 확인:

```powershell
dotnet list package
```

---

### 8.2. .NET Framework-Style + packages.config Only

> 오타 주의: 일반적으로 `Framework-Style`이 아니라 `Framework-Style`이다.

구형 `.NET Framework Style`에서 packages.config 방식 사용 예:

```xml
<!-- packages.config -->
<packages>
  <package id="Newtonsoft.Json" version="13.0.1" targetFramework="net481" />
</packages>
```

설치:

```powershell
Install-Package Newtonsoft.Json -Version 13.0.1 -ProjectName CSharp
```

삭제:

```powershell
Uninstall-Package Newtonsoft.Json -ProjectName CSharp
```

버전 변경:

```powershell
Update-Package Newtonsoft.Json -Version 13.0.4 -ProjectName CSharp
```

복원:

```powershell
nuget restore YourSolution.sln
```

또는 Visual Studio:

```text
솔루션 우클릭 → Restore NuGet Packages
```

주의:

```text
packages.config의 version을 직접 수정하지 말 것.
버전 변경은 NuGet 패키지 관리 UI 또는 Update-Package를 사용할 것.
```

---

### 8.3. 중앙 패키지 관리 (CPM)

`Directory.Packages.props`:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
  </ItemGroup>
</Project>
```

`.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" />
</ItemGroup>
```

복원:

```powershell
dotnet restore
```

목록 확인:

```powershell
dotnet list package
```

패키지 추가 시 주의:

```text
NuGet UI 또는 dotnet add package가 .csproj에 Version을 넣으면
CPM에서는 NU1008 오류가 발생할 수 있다.
```

올바른 형태:

```xml
<PackageReference Include="Newtonsoft.Json" />
```

그리고:

```xml
<PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
```

---

## 9. Trouble Shooting

### 9.1. NU1008 오류

오류:

```text
중앙 패키지 버전 관리를 사용하는 프로젝트에서는
PackageReference 항목에 대한 버전이 아니라
PackageVersion 항목에 대한 버전을 정의해야 합니다.
```

원인:

```xml
<PackageReference Include="x" Version="x.x" />
```

또는:

```xml
<PackageReference Include="x">
  <Version>x.x</Version>
</PackageReference>
```

해결:

```xml
<PackageReference Include="x" />
```

그리고 `Directory.Packages.props`에:

```xml
<PackageVersion Include="x" Version="x.x" />
```

---

### 9.3. NU1504 중복 PackageReference 경고

원인:

```xml
<PackageReference Include="x" />
<PackageReference Include="x" Version="x.x" />
```

해결:

CPM 사용 시:

```xml
<PackageReference Include="x" />
```

일반 PackageReference 사용 시:

```xml
<PackageReference Include="x" Version="x.x" />
```

---

### 9.4. RestoreProjectStyle 추가 후 NuGet 패키지 관리 UI내의 설치 목록이 불일치한 현상

기존 패키지가 `packages.config` 방식으로 설치되어 있는데:

```xml
<RestoreProjectStyle>PackageReference</RestoreProjectStyle>
```

만 추가하면 NuGet UI는 `PackageReference` 기준으로 설치 목록을 본다.

그런데 실제 패키지는 `packages.config` 방식으로 설치되어 있으므로 설치됨 목록이 비어 보일 수 있다.

해결:

```text
packages.config 방식을 유지할 경우
  → RestoreProjectStyle 제거

PackageReference 방식으로 전환할 경우
  → packages.config, Reference + HintPath, Import 제거
  → PackageReference 추가
  → NuGet 패키지 관리 UI를 통해 설치 과정 진행
```

---

### 9.5. NuGet 패키지 설치 후에 packages.config가 자동으로 생성되지 않는 현상

`packages.config`는 프로젝트를 열었다고 자동으로 생기지 않는다.

생성 조건:

```text
1. 구형 .NET Framework 프로젝트
2. PackageReference 없음
3. RestoreProjectStyle=PackageReference 없음
4. NuGet 패키지 관리 기능으로 패키지 설치
```

`.csprojt` 내에 `PackageReference` 속성이 존재 한다면 
`.csprojt` 파일이 있는 경로에 `packages.config` 파일이 생기지 않는다.

---

### 9.6. packages.config 내의 내용을 직접 수정 후 NuGet 패키지 관리 UI내의 정보가 불일치한 현상

원인:

```text
packages.config의 version과
.csproj의 HintPath 버전 경로가 달라졌기 때문
```

예:

```xml
<!-- packages.config -->
<package id="x" version="3.6.4" targetFramework="net481" />
```

```xml
<!-- csproj -->
<HintPath>..\packages\x.4.2.2\lib\net462\x.dll</HintPath>
```

해결:

```powershell
Update-Package x -Version 3.6.4 -ProjectName CSharp
```

---

### 9.7. 상위 CPM은 켜져 있지만 특정 프로젝트만 일반 PackageReference로 설정 하기

상위 `Directory.Packages.props`에 다음이 있어도:

```xml
<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
```

특정 프로젝트에서 다음을 설정하면 CPM에서 제외할 수 있다.

```xml
<PropertyGroup>
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>
```

그 프로젝트는 일반 `PackageReference` 방식으로 동작한다.

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

이 상태에서도 NuGet 패키지 관리 UI, `dotnet add package`, `dotnet remove package`, `dotnet restore`를 모두 사용할 수 있다.

단, 이 프로젝트에서는 `PackageReference`에 반드시 `Version`을 직접 써야 한다.

---

### 9.8. SDK-Style 프로젝트에서 Reference + HintPath를 사용할 수 있나?

가능은 하다.

예:

```xml
<ItemGroup>
  <Reference Include="MyCompany.Common">
    <HintPath>..\libs\MyCompany.Common.dll</HintPath>
  </Reference>
</ItemGroup>
```

하지만 SDK-style 프로젝트에서 NuGet 패키지를 관리하는 용도로는 `Reference + HintPath`를 사용하지 않는 것이 일반적이다.

NuGet 패키지는 `PackageReference`로 관리한다.

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
```

---

### 9.9. Directory.Packages.props가 있으면 Solution/packages 경로를 기본적으로 사용하게 되나?

아니다.

```text
Directory.Packages.props
  → PackageReference / CPM 방식에서 사용하는 중앙 버전 관리 파일

Solution/packages
  → packages.config 파일 활용 방식에서 주로 사용하는 패키지 저장 폴더
```

`CPM / PackageReference` 방식에서는 기본적으로 `전역 NuGet 캐시 경로`를 사용한다.

```text
C:\Users\<UserName>\.nuget\packages
```

패키지 저장 경로를 바꾸고 싶으면 `nuget.config`의 `globalPackagesFolder`를 사용한다.

```xml
<add key="globalPackagesFolder" value=".nuget\packages" />
```
