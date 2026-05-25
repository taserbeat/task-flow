import { useState, useEffect, useMemo } from "react";

interface BoardFormProps {
  initialData?: { name: string };
  onSubmit: (formData: { name: string }) => Promise<void>;
  isLoading?: boolean;
}

/** ボード作成・編集フォーム */
const BoardForm = ({
  initialData,
  onSubmit,
  isLoading = false,
}: BoardFormProps) => {
  const [formData, setFormData] = useState({
    name: "",
  });

  useEffect(() => {
    if (initialData) {
      setFormData({
        name: initialData.name || "",
      });
    }
  }, [initialData]);

  const hasChanges = useMemo(() => {
    if (!initialData) return true;
    return formData.name !== (initialData.name || "");
  }, [formData, initialData]);

  const isFormValid = useMemo(() => {
    return formData.name.trim() !== "";
  }, [formData]);

  const isSubmitDisabled = isLoading || !hasChanges || !isFormValid;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit({ name: formData.name });
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-xl font-semibold text-gray-900">
            {initialData ? "ボード情報編集" : "新規ボード作成"}
          </h2>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-6">
          <div className="space-y-4">
            <div>
              <label
                htmlFor="name"
                className="block text-sm font-medium text-gray-700 mb-2"
              >
                ボード名
              </label>
              <input
                id="name"
                name="name"
                type="text"
                value={formData.name}
                onChange={handleChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                placeholder="ボード名を入力"
              />
            </div>
          </div>

          <div className="flex justify-end pt-4 border-t border-gray-200">
            <button
              type="submit"
              disabled={isSubmitDisabled}
              className={`px-6 py-2 rounded-md font-medium transition-colors ${
                isSubmitDisabled
                  ? "bg-gray-200 text-gray-400 cursor-not-allowed"
                  : "bg-blue-600 text-white hover:bg-blue-700 cursor-pointer"
              }`}
            >
              {isLoading ? "保存中..." : initialData ? "更新" : "作成"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default BoardForm;
