#!/bin/bash

<<COMMENTOUT

COMMENTOUT

# -------------------------------------------------------------------------------------------

readonly SCRIPT_EXECUTED_DIR=$(pwd)
readonly SCRIPT_DIR=$(
  cd $(dirname $0)
  pwd
)

readonly TARGETS=("all" "frontend" "backend")

# -------------------------------------------------------------------------------------------

function build_frontend() {
  cd "${SCRIPT_DIR}/frontend"

  yarn install
  yarn build
}

function build_backend() {
  cd ${SCRIPT_DIR}

  if [ -e ./build/ ]; then
    rm -rf build/
  fi

  dotnet restore

  readonly APP_NAME="TaskFlow"

  # .csprojの名前
  readonly TARGET_PROJECT_NAME="Web"

  readonly BACKEND_DIR="${SCRIPT_DIR}/backend"
  cd ${BACKEND_DIR}

  # .csprojが存在するディレクトリパス
  readonly CSPROJECT_DIR="${BACKEND_DIR}/${TARGET_PROJECT_NAME}"

  # .csprojのファイルパス
  readonly CSPROJECT_PATH="${CSPROJECT_DIR}/${TARGET_PROJECT_NAME}.csproj"

  # ビルドの成果物を出力するディレクトリ
  readonly BUILD_DEST_DIR_ROOT="${SCRIPT_DIR}/build"

  # zip圧縮で出力するディレクトリ
  readonly ZIP_DEST_DIR="${BUILD_DEST_DIR_ROOT}/zip"

  # https://docs.microsoft.com/ja-jp/dotnet/core/rid-catalog
  readonly RUNTIMES=("linux-x64" "osx-x64" "win-x64")

  # Directory.Build.props が存在するディレクトリ
  readonly PROPS_DIR="${SCRIPT_DIR}/backend"
  readonly PROPS_FILENAME="Directory.Build.props"

  cd ${PROPS_DIR}

  readonly VERSION=$(
    cat ${PROPS_FILENAME} |
      grep -E "<Version>([0-9A-Za-z_.\-]+?)</Version>" |
      sed -E "s/[ \f\n\r\t]+.?<Version>([0-9A-Za-z_.\-]+.?)<\/Version>.*$/\1/"
  )

  for runtime in "${RUNTIMES[@]}"; do
    cd ${CSPROJECT_DIR}

    # ビルドの出力先ディレクトリ
    build_dest_dir="${BUILD_DEST_DIR_ROOT}/${runtime}/${APP_NAME}"

    dotnet publish ${CSPROJECT_PATH} -c Release -o ${build_dest_dir} --runtime ${runtime} --self-contained

    # 以下、必要なファイルをfrontendフォルダからコピー
    privateroot_dir="${build_dest_dir}/privateroot"
    wwwroot_dir="${build_dest_dir}/wwwroot"
    frontend_build_dir="${SCRIPT_DIR}/frontend/dist"

    mkdir -p ${privateroot_dir}
    mkdir -p ${wwwroot_dir}

	cp -R "${frontend_build_dir}/." ${privateroot_dir}

    mkdir -p ${ZIP_DEST_DIR}

    if [ ! -e ${build_dest_dir} ]; then
      continue
    fi

    # ビルドで出力したディレクトリを一時的にコピーした後、zip圧縮する
    cd ${ZIP_DEST_DIR}

    app_tag="${APP_NAME}_${runtime}_v${VERSION}"
    tmp_dir_path="./${app_tag}/"
    cp -r ${build_dest_dir} ${tmp_dir_path}

    zip_dest_path="./${app_tag}.zip"
    zip -q -r ${zip_dest_path} ${tmp_dir_path}

    rm -r ${tmp_dir_path}

  done

}

function build_all() {
  build_frontend
  build_backend
}

# -------------------------------------------------------------------------------------------

target=${TARGETS[0]}

# コマンドライン引数の解析
if [ $# -gt 1 ]; then
  echo "[エラー] 引数は1個以下にして下さい"
  exit 1
elif [ $# -eq 1 ]; then
  if $(echo ${TARGETS[@]} | grep -q $1); then
    target=$1
  else
    echo "[エラー] 引数に '$1' は指定できません"
    exit 1
  fi
fi

# アクションの実行
if [ ${target} == "all" ]; then
  echo "ターゲット: '${target}' でビルドします"
  build_all
elif [ ${target} == "frontend" ]; then
  echo "ターゲット: '${target}' でビルドします"
  build_frontend
elif [ ${target} == "backend" ]; then
  echo "ターゲット: '${target}' でビルドします"
  build_backend
else

  echo "ターゲット: '${target}' は存在しません"
  cd ${SCRIPT_EXECUTED_DIR}
  exit 1
fi

cd ${SCRIPT_EXECUTED_DIR}
