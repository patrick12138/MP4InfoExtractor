# MP4InfoExtractor

[English](./README.md) | [简体中文](./README.zh-CN.md)

MP4InfoExtractor is a Windows desktop tool (WPF, .NET Framework 4.5) for scanning video files and extracting song metadata from filename conventions, then exporting results to CSV.

## About

- Purpose: Batch parse music-video filenames into structured song data.
- Platform: Windows (.NET Framework 4.5).
- Output: CSV report for downstream sorting, cataloging, or import.

## Features

- Recursively scans folders for common video formats:
  - `*.mp4`, `*.mpg`, `*.mpeg`, `*.avi`, `*.mkv`, `*.mov`, `*.wmv`, `*.flv`, `*.webm`
- Parses naming patterns like:
  - `Artist - SongTitle--Gender/Group--Type--Language--Region--Quality`
- Includes text helpers for abbreviation/pinyin generation support.
- Exports parsed results to CSV.

## Project Structure

- `MP4InfoExtractor/`: Main WPF application source.
- `packages.config`: NuGet dependency declarations.
- `release/`: Local release artifacts (ignored in Git).

## Prerequisites

- Windows
- Visual Studio (2019+ recommended)
- .NET Framework 4.5 Developer Pack

## Build

1. Open `MP4InfoExtractor/MP4InfoExtractor.csproj` in Visual Studio.
2. Restore NuGet packages.
3. Build in `Debug` or `Release` mode.

## Usage

1. Launch the app.
2. Click **Browse** to select a folder containing video files.
3. Click **Scan** to parse songs.
4. Click **Export CSV** to save results.

## Notes

- This repository ignores build outputs and local artifacts by default.
- If you clone fresh, restore NuGet packages before building.
